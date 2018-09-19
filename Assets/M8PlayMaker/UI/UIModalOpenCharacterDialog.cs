using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Opens a character dialog. Will not finish until dialog calls back action.")]
    public class UIModalOpenCharacterDialog : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("If empty, will use the default modal reference name")]
        public FsmString modalRef;

        [HutongGames.PlayMaker.Tooltip("If name, text, and choices are references to the localizer. default = true")]
        public FsmBool isLocalize;

        [HutongGames.PlayMaker.Tooltip("Close the dialog after player selects a choice, or just clicks on the dialog")]
        public FsmBool closeOnAction;

        [HutongGames.PlayMaker.Tooltip("The event to set to after a choice is selected, or after click on dialog")]
        public FsmEvent actionEvent;

        public FsmString name;
        
        [RequiredField]
        public FsmString text;
                
        public FsmString portrait;

        [UIHint(UIHint.Variable)]
        public FsmInt choiceOutput;

        public string[] choices;
        
        public override void Reset() {
            base.Reset();

            modalRef = null;
            name = null;
            text = null;
            portrait = null;
            choices = null;

            choiceOutput = null;

            isLocalize = true;
            closeOnAction = false;

            actionEvent = null;
        }

        string GetModalRef() {
            return modalRef.IsNone ? UIModalCharacterDialog.defaultModalRef : modalRef.Value;
        }

	    // Code that runs on entering the state.
	    public override void OnEnter()
	    {
            UIModalCharacterDialog dlg;

            dlg = UIModalCharacterDialog.Open(
                    isLocalize.Value,
                    GetModalRef(),
                    text.Value,
                    name.Value,
                    portrait.Value,
                    choices);

            if(dlg != null) {
                dlg.actionCallback += OnAction;
            }
            else {
                Finish();
            }
	    }

        // Code that runs every frame.
        public override void OnUpdate()
	    {
		
	    }

	    // Code that runs when exiting the state.
	    public override void OnExit()
	    {
            if(UIModalManager.instance != null) {
                UIModalCharacterDialog dlg = UIModalManager.instance.ModalGetController<UIModalCharacterDialog>(
                    GetModalRef());

                if(dlg != null) {
                    dlg.actionCallback -= OnAction;
                }
            }
	    }

        void OnAction(int choiceIndex) {
            //save to variable
            if(!choiceOutput.IsNone) {
                choiceOutput.Value = choiceIndex;
            }

            //close?
            if(closeOnAction.Value && UIModalManager.instance.ModalGetTop() == GetModalRef()) {
                UIModalManager.instance.ModalCloseTop();
            }

            //envoke event
            if(!FsmEvent.IsNullOrEmpty(actionEvent)) {
                Fsm.Event(actionEvent);
            }

            Finish();
        }
    }
}

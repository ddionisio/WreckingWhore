using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Opens a character dialog. Will not finish until dialog calls back action. This is for localized version. The last text will display the choices.")]
    public class UIModalOpenCharacterDialogSequenceLocalized : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("If empty, will use the default modal reference name")]
        public FsmString modalRef;

        [HutongGames.PlayMaker.Tooltip("Close the dialog after player selects a choice, or just clicks on the dialog")]
        public FsmBool closeOnAction;

        [HutongGames.PlayMaker.Tooltip("The event to set to after a choice is selected, or after click on dialog")]
        public FsmEvent actionEvent;

        public FsmString name;
        
        [RequiredField]
        public FsmString textPrefix;

        [RequiredField]
        public FsmInt textIndexStart;

        [RequiredField]
        public FsmInt textIndexEnd;
                
        public FsmString portrait;

        [UIHint(UIHint.Variable)]
        public FsmInt choiceOutput;

        public string[] choices;

        private int mCurIndex = 0;
        
        public override void Reset() {
            base.Reset();

            modalRef = null;
            name = null;

            textPrefix = null;
            textIndexStart = 0;
            textIndexEnd = 0;

            portrait = null;
            choices = null;

            choiceOutput = null;

            closeOnAction = false;

            actionEvent = null;
        }

        string GetModalRef() {
            return modalRef.IsNone ? UIModalCharacterDialog.defaultModalRef : modalRef.Value;
        }

        //true = success
        bool SetDialog(bool addCallback) {
            UIModalCharacterDialog dlg;

            dlg = UIModalCharacterDialog.Open(
                    true,
                    GetModalRef(),
                    textPrefix.Value + mCurIndex.ToString(),
                    name.Value,
                    portrait.Value,
                    mCurIndex == textIndexEnd.Value ? choices : null);

            if(dlg != null) {
                if(addCallback)
                    dlg.actionCallback += OnAction;

                return true;
            }

            return false;
        }

	    // Code that runs on entering the state.
	    public override void OnEnter()
	    {
            mCurIndex = textIndexStart.Value;

            if(!SetDialog(true)) {
                Finish();
            }
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
            if(mCurIndex == textIndexEnd.Value) {
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
            else {
                //go to next
                mCurIndex++;

                if(!SetDialog(false)) {
                    Finish();
                }
            }
        }
    }
}

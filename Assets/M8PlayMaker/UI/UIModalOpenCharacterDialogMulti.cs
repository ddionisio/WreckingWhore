using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Opens a character dialog with a series of texts, last text will show choices. Will not finish until dialog calls back action.")]
    public class UIModalOpenCharacterDialogMulti : FsmStateAction
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
                
        public FsmString portrait;

        [UIHint(UIHint.Variable)]
        public FsmInt choiceOutput;

        public string[] texts;
        public string[] choices;

        private int mCurIndex;
        
        public override void Reset() {
            base.Reset();

            modalRef = null;
            name = null;
            texts = null;
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

        //true = success
        bool SetDialog(bool addCallback) {
            if(texts != null && texts.Length > 0) {
                UIModalCharacterDialog dlg;

                dlg = UIModalCharacterDialog.Open(
                        isLocalize.Value,
                        GetModalRef(),
                        texts[mCurIndex],
                        name.Value,
                        portrait.Value,
                        mCurIndex == texts.Length - 1 ? choices : null);

                if(dlg != null) {
                    if(addCallback)
                        dlg.actionCallback += OnAction;

                    return true;
                }
            }

            return false;
        }

	    // Code that runs on entering the state.
	    public override void OnEnter()
	    {
            mCurIndex = 0;
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
            if(mCurIndex == texts.Length - 1) {
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
                mCurIndex++;
                if(!SetDialog(false))
                    Finish();
            }
        }
    }
}

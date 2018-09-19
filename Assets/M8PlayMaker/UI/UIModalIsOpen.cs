using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Check if given UI is currently open.")]
    public class UIModalIsOpen : FsmStateAction {
        public FsmString modal;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public bool everyFrame;

        public override void Reset() {
            modal = null;
            isTrue = null;
            isFalse = null;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            Finish();

            DoCheck();
            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            if(UIModalManager.instance != null) {
                if(UIModalManager.instance.ModalIsInStack(modal.Value)) {
                    Fsm.Event(isTrue);
                }
                else {
                    Fsm.Event(isFalse);
                }
            }
            else {
                Finish();
            }
        }

        public override string ErrorCheck() {
            if(everyFrame &&
                FsmEvent.IsNullOrEmpty(isTrue) &&
                FsmEvent.IsNullOrEmpty(isFalse))
                return "Action sends no events!";
            return "";
        }
    }
}
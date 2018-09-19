using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate FSM")]
    [HutongGames.PlayMaker.Tooltip("Check if the last event matches given event.")]
    public class CheckLastFSMEvent : FsmStateAction {
        [RequiredField]
        public FsmEvent checkEvent;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public override void Reset() {
            base.Reset();

            checkEvent = null;
            isTrue = null;
            isFalse = null;
        }
        // Code that runs on entering the state.
        public override void OnEnter() {
            //Fsm.LastTransition
            if(Fsm.LastTransition.EventName == checkEvent.Name) {
                if(!FsmEvent.IsNullOrEmpty(isTrue))
                    Fsm.Event(isTrue);
            }
            else {
                if(!FsmEvent.IsNullOrEmpty(isFalse))
                    Fsm.Event(isFalse);
            }

            Finish();
        }

        public override string ErrorCheck() {
            if(FsmEvent.IsNullOrEmpty(isTrue) &&
                FsmEvent.IsNullOrEmpty(isFalse))
                return "Action sends no events!";
            return "";
        }
    }
}
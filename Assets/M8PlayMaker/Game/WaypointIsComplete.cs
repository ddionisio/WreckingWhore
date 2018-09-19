using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Check if waypoint is done. Use this after WaypointNext")]
    public class WaypointIsComplete : FSMActionComponentBase<WaypointData> {
        [UIHint(UIHint.Variable)]
        public FsmBool result;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            result = null;
            isTrue = null;
            isFalse = null;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            DoCheck();

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            if(!result.IsNone)
                result = mComp.isDone;

            if(mComp.isDone)
                Fsm.Event(isTrue);
            else
                Fsm.Event(isFalse);
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
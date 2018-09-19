using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Move waypoint backwards.")]
    public class WaypointPrevious : FSMActionComponentBase<WaypointData> {
        public FsmEvent isFirst;

        public override void Reset() {
            base.Reset();

            isFirst = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Prev();

            if(mComp.counter == 0)
                Fsm.Event(isFirst);

            Finish();
        }
    }
}
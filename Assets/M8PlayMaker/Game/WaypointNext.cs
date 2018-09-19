using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Move waypoint to the next.")]
    public class WaypointNext : FSMActionComponentBase<WaypointData> {
        public FsmEvent isDone;

        public override void Reset() {
            base.Reset();

            isDone = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Next();

            if(mComp.isDone) {
                Fsm.Event(isDone);
            }

            Finish();
        }
    }
}
using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    public class WaypointMoverPause : FSMActionComponentBase<WaypointMover> {

        public FsmBool pause;

        public override void Reset() {
            base.Reset();

            pause = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.pause = pause.Value;

            Finish();
        }
    }
}

using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Reset waypoint to beginning.")]
    public class WaypointReset : FSMActionComponentBase<WaypointData> {
        public override void Reset() {
            base.Reset();
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Restart();

            Finish();
        }
    }
}
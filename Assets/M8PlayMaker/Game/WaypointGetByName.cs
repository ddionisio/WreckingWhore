using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Get a waypoint from the waypoint manager and set the WaypointData. Do this sparingly, ie. on start or cache the ones you need.")]
    public class WaypointGetByName : FSMActionComponentBase<WaypointData> {
        [RequiredField]
        public FsmString waypoint;

        public FsmEvent onInvalid;

        public override void Reset() {
            base.Reset();

            waypoint = null;
            onInvalid = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            if(!mComp.Apply(waypoint.Value)) {
                LogWarning("Waypoint: " + waypoint.Value + " was not found!");

                if(!FsmEvent.IsNullOrEmpty(onInvalid))
                    Fsm.Event(onInvalid);
            }

            Finish();
        }
    }
}
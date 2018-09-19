using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Waypoint")]
    [HutongGames.PlayMaker.Tooltip("Get a waypoint from WaypointData and put it in a Vector/GameObject")]
    public class WaypointGetCurrent : FSMActionComponentBase<WaypointData> {
        [UIHint(UIHint.Variable)]
        public FsmVector3 toVector;

        [UIHint(UIHint.Variable)]
        public FsmGameObject toGO;

        public override void Reset() {
            base.Reset();

            toVector = null;
            toGO = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            //Debug.Log("Waypoint: " + mComp.waypoint + " index: " + mComp.curInd);

            if(!toVector.IsNone)
                toVector.Value = mComp.current.position;

            if(!toGO.IsNone)
                toGO.Value = mComp.current.gameObject;

            Finish();
        }
    }
}
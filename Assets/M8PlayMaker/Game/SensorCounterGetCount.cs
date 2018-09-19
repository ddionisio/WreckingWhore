using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Triggers")]
    [HutongGames.PlayMaker.Tooltip("Get count from SensorCounter")]
    public class SensorCounterGetCount : FSMActionComponentBase<SensorCounter> {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            output = null;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            //Debug.Log("Waypoint: " + mComp.waypoint + " index: " + mComp.curInd);
            output.Value = mComp.count;

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            output.Value = mComp.count;
        }
    }
}
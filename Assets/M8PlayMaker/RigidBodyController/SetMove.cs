using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker.RigidBodyControllerLib {
    [ActionCategory("Mate RigidBodyController")]
    [HutongGames.PlayMaker.Tooltip("Set either or both the forward and side move scale of the RigidBodyController. Normally you want it either 0 or 1, such that it moves based on the force value.")]
    public class SetMove : FSMActionComponentBase<RigidBodyController> {
        public FsmFloat forward;
        public FsmFloat side;

        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            forward = 0.0f;
            side = 0.0f;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            DoApply();

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoApply();
        }

        void DoApply() {
            mComp.moveForward = forward.Value;
            mComp.moveSide = side.Value;
        }
    }
}

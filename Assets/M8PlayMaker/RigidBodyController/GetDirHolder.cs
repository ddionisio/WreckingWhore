using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker.RigidBodyControllerLib {
    [ActionCategory("Mate RigidBodyController")]
    [HutongGames.PlayMaker.Tooltip("Get the direction holder of the RigidBodyController")]
    public class GetDirHolder : FSMActionComponentBase<RigidBodyController> {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject output;

        public override void Reset() {
            base.Reset();

            output = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            output.Value = mComp.dirHolder.gameObject;

            Finish();
        }

    }
}

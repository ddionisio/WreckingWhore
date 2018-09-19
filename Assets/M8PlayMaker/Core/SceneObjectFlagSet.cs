using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer")]
    public class SceneObjectFlagSet : FSMActionComponentBase<SceneSerializer> {
        [RequiredField]
        public FsmString name;

        [RequiredField]
        public FsmInt bit;

        [RequiredField]
        public FsmBool val;

        public FsmBool persistent;

        public override void Reset() {
            base.Reset();

            name = null;
            bit = null;
            val = null;
            persistent = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            mComp.SetFlag(name.Value, bit.Value, val.Value, persistent.Value);

            Finish();
        }
    }
}

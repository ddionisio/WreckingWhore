using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer")]
    public class SceneObjectGetID : FSMActionComponentBase<SceneSerializer> {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public override void Reset() {
            base.Reset();

            output = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            output = mComp.id;

            Finish();
        }
    }
}

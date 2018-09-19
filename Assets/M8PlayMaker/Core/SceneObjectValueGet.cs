using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer")]
    public class SceneObjectValueGet : FSMActionComponentBase<SceneSerializer> {
        [RequiredField]
        public FsmString name;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVar toValue;

        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            name = null;
            toValue = null;
            everyFrame = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            toValue.intValue = mComp.GetValue(name.Value);
            toValue.floatValue = mComp.GetValueFloat(name.Value);

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            toValue.intValue = mComp.GetValue(name.Value);
            toValue.floatValue = mComp.GetValueFloat(name.Value);
        }
    }
}

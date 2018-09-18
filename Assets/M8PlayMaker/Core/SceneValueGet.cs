using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    public class SceneValueGet : FsmStateAction {
        [RequiredField]
        public FsmString name;

        public bool global;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVar toValue;

        public bool everyFrame;

        public override void Reset() {
            name = null;
            global = false;
            toValue = null;
            everyFrame = false;
        }

        public override void OnEnter() {
            if(SceneState.instance != null) {
                toValue.intValue = global ? SceneState.instance.GetGlobalValue(name.Value) : SceneState.instance.GetValue(name.Value);
                toValue.floatValue = global ? SceneState.instance.GetGlobalValueFloat(name.Value) : SceneState.instance.GetValueFloat(name.Value);
            }

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            toValue.intValue = global ? SceneState.instance.GetGlobalValue(name.Value) : SceneState.instance.GetValue(name.Value);
            toValue.floatValue = global ? SceneState.instance.GetGlobalValueFloat(name.Value) : SceneState.instance.GetValueFloat(name.Value);
        }
    }
}

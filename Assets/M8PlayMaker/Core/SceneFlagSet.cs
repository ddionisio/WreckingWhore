using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    public class SceneFlagSet : FsmStateAction {
        [RequiredField]
        public FsmString name;

        public bool global;

        [RequiredField]
        public FsmInt bit;

        [RequiredField]
        public FsmBool val;

        public FsmBool persistent;

        public override void Reset() {
            name = null;
            global = false;
            bit = null;
            val = null;
            persistent = false;
        }

        public override void OnEnter() {
            if(SceneState.instance != null) {
                if(global)
                    SceneState.instance.SetGlobalFlag(name.Value, bit.Value, val.Value, persistent.Value);
                else
                    SceneState.instance.SetFlag(name.Value, bit.Value, val.Value, persistent.Value);
            }

            Finish();
        }
    }
}

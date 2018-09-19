using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Load a scene by level index.")]
    public class SceneLoadLevel : FsmStateAction {
        [RequiredField]
        public FsmInt level;

        public override void Reset() {
            level = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.LoadLevel(level.Value);

            Finish();
        }
    }
}
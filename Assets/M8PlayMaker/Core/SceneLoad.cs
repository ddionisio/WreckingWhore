using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Load a new scene by name.")]
    public class SceneLoad : FsmStateAction {
        [RequiredField]
        public FsmString scene;

        public override void Reset() {
            scene = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.LoadScene(scene.Value);

            Finish();
        }
    }
}
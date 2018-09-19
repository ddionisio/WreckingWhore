using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Reload the current scene.")]
    public class SceneReload : FsmStateAction {

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.Reload();

            Finish();
        }
    }
}
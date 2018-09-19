using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Resume the scene.")]
    public class SceneResume : FsmStateAction {

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.Resume();

            Finish();
        }
    }
}
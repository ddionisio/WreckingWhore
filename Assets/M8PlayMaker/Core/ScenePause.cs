using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Pause the scene.")]
    public class ScenePause : FsmStateAction {

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.Pause();

            Finish();
        }
    }
}
using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Do a broadcast to all objects in the scene hierarchy.")]
    public class SceneBroadcast : FsmStateAction {
        [RequiredField]
        public FsmString func;

        public FsmObject par;

        public override void Reset() {
            func = null;
            par = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.sceneManager.BroadcastMessage(func.Value, par.Value, SendMessageOptions.DontRequireReceiver);

            Finish();
        }


    }
}
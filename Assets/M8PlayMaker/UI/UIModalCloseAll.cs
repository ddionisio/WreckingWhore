using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Close all modals.")]
    public class UIModalCloseAll : FsmStateAction {

        // Code that runs on entering the state.
        public override void OnEnter() {
            UIModalManager.instance.ModalCloseAll();

            Finish();
        }
    }
}
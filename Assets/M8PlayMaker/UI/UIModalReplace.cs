using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Close all modals and open a new one.")]
    public class UIModalReplace : FsmStateAction {
        public FsmString modal;

        public override void Reset() {
            modal = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            UIModalManager.instance.ModalReplace(modal.Value);
            Finish();
        }
    }
}
using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Open a UI Modal")]
    public class UIModalOpen : FsmStateAction {
        public FsmString modal;

        public override void Reset() {
            modal = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            UIModalManager.instance.ModalOpen(modal.Value);

            Finish();
        }
    }
}
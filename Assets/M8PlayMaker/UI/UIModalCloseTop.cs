using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Close the top modal.")]
    public class UIModalCloseTop : FsmStateAction {

        // Code that runs on entering the state.
        public override void OnEnter() {
            UIModalManager.instance.ModalCloseTop();

            Finish();
        }
    }
}
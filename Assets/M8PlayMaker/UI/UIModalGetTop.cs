using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate UI")]
    [HutongGames.PlayMaker.Tooltip("Get the current active modal's name. Empty if no modal is active.")]
    public class UIModalGetTop : FsmStateAction {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString toValue;

        public override void Reset() {
            toValue = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            toValue.Value = UIModalManager.instance.ModalGetTop();

            Finish();
        }
    }
}
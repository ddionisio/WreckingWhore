using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Input")]
    [HutongGames.PlayMaker.Tooltip("Get the axis value of given player's input action.  Note: Refer to the generated InputAction.cs for action reference.")]
    public class InputGetAxis : FsmStateAction {
        public FsmInt player;
        public FsmInt action;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat toValue;

        public override void Reset() {
            player = null;
            action = null;
            toValue = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            toValue.Value = Main.instance.input.GetAxis(player.Value, action.Value);

            Finish();
        }
    }
}
using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate String")]
    [HutongGames.PlayMaker.Tooltip("Set a randomized number on given string. Make sure the format string has a {0}")]
    public class StringNumber : FsmStateAction {
        public FsmInt value;

        [RequiredField]
        public FsmString format;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString output;

        public override void Reset() {
            value = 0;
            format = null;
            output = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            output.Value = string.Format(format.Value, value.Value);

            Finish();
        }
    }
}

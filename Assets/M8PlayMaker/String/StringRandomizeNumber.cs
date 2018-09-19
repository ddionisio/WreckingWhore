using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate String")]
    [HutongGames.PlayMaker.Tooltip("Set a randomized number on given string. Make sure the format string has a {0}")]
    public class StringRandomizeNumber : FsmStateAction {
        public FsmInt min;
        public FsmInt max;

        [RequiredField]
        public FsmString format;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString output;

        public override void Reset() {
            min = 0;
            max = 0;
            format = null;
            output = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            output.Value = string.Format(format.Value, Random.Range(min.Value, max.Value + 1));

            Finish();
        }
    }
}

using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Animator")]
    [HutongGames.PlayMaker.Tooltip("Get the current take that is playing.")]
    public class AMGetCurrentTake : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have an AnimatorData component attached.")]
        [CheckForComponent(typeof(AnimatorData))]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString output;

        private AnimatorData aData;
        private void InitData() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            aData = go.GetComponent<AnimatorData>();
        }

        public override void Reset() {
            output = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            InitData();
            output.Value = aData.takeName;
            Finish();
        }
    }
}

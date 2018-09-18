using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Animator")]
    [Tooltip("Reverse animator timeline. This will only work if there is a current Take set to AnimatorData (via Play)")]
    public class AMReverse : FsmStateAction {
        public enum Type {
            Toggle,
            True,
            False
        }

        [RequiredField]
        [Tooltip("The Game Object to work with. NOTE: The Game Object must have an AnimatorData component attached.")]
        [CheckForComponent(typeof(AnimatorData))]
        public FsmOwnerDefault gameObject;

        public Type mode = Type.Toggle;

        private AnimatorData aData;
        private void InitData() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            aData = go.GetComponent<AnimatorData>();
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            InitData();

            switch(mode) {
                case Type.Toggle:
                    aData.Reverse();
                    break;

                case Type.True:
                    aData.isReversed = true;
                    break;

                case Type.False:
                    aData.isReversed = false;
                    break;
            }

            Finish();
        }
    }
}

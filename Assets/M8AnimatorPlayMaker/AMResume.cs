using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Animator")]
    [HutongGames.PlayMaker.Tooltip("Resume animator timeline. This will only work if there is a current Take set to AnimatorData (via Play)")]
    public class AMResume : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have an AnimatorData component attached.")]
        [CheckForComponent(typeof(AnimatorData))]
        public FsmOwnerDefault gameObject;

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
            aData.Resume();
            Finish();
        }
    }
}

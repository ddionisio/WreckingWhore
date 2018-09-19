using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Animator")]
    [HutongGames.PlayMaker.Tooltip("Check to see if animator timeline is playing.")]
    public class AMCheckPlaying : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have an AnimatorData component attached.")]
        [CheckForComponent(typeof(AnimatorData))]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public FsmBool everyFrame;

        private AnimatorData aData;
        private void InitData() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            aData = go.GetComponent<AnimatorData>();
        }

        public override void Reset() {
            isTrue = null;
            isFalse = null;
            storeResult = null;
            everyFrame = null;
        }

        public override void OnEnter() {
            InitData();
            DoCheck();
            if(!everyFrame.Value)
                Finish();
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            bool playing = aData.isPlaying;

            if(!storeResult.IsNone)
                storeResult.Value = playing;

            if(playing)
                Fsm.Event(isTrue);
            else
                Fsm.Event(isFalse);
        }

        public override string ErrorCheck() {
            if(everyFrame.Value &&
                FsmEvent.IsNullOrEmpty(isTrue) &&
                FsmEvent.IsNullOrEmpty(isFalse))
                return "Action sends no events!";
            return "";
        }
    }
}

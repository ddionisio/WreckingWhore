using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer")]
    public class SceneObjectFlagCheck : FSMActionComponentBase<SceneSerializer> {
        [RequiredField]
        public FsmString name;

        [RequiredField]
        public FsmInt bit;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            name = null;
            bit = null;
            isTrue = null;
            isFalse = null;
            everyFrame = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            DoCheck();
            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            if(mComp.CheckFlag(name.Value, bit.Value)) {
                Fsm.Event(isTrue);
            }
            else {
                Fsm.Event(isFalse);
            }
        }

        public override string ErrorCheck() {
            if(everyFrame &&
                FsmEvent.IsNullOrEmpty(isTrue) &&
                FsmEvent.IsNullOrEmpty(isFalse))
                return "Action sends no events!";
            return "";
        }
    }
}

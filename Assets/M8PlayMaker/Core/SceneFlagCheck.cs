using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    public class SceneFlagCheck : FsmStateAction {
        [RequiredField]
        public FsmString name;

        public bool global;

        [RequiredField]
        public FsmInt bit;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public bool everyFrame;

        public override void Reset() {
            name = null;
            bit = null;
            isTrue = null;
            isFalse = null;
            everyFrame = false;
        }

        public override void OnEnter() {
            if(SceneState.instance != null) {
                DoCheck();
                if(!everyFrame)
                    Finish();
            }
            else {
                Finish();
            }
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            if(SceneState.instance != null) {
                if(global ? SceneState.instance.CheckGlobalFlag(name.Value, bit.Value) : SceneState.instance.CheckFlag(name.Value, bit.Value)) {
                    Fsm.Event(isTrue);
                }
                else {
                    Fsm.Event(isFalse);
                }
            }
            else {
                Finish();
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

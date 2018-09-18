using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    public class SceneValueListen : FsmStateAction {
        [RequiredField]
        public FsmString name;

        public bool global;

        [UIHint(UIHint.Variable)]
        public FsmInt iValueOut;

        [UIHint(UIHint.Variable)]
        public FsmFloat fValueOut;

        public FsmEvent changeEvent;

        public override void Reset() {
            name = null;
            global = false;

            iValueOut = null;
            fValueOut = null;

            changeEvent = null;
        }

        public override void OnEnter() {
            if(SceneState.instance != null) {
                SceneState.instance.onValueChange += StateCallback;
            }
            else {
                Finish();
            }
        }

        public override void OnExit() {
            if(SceneState.instance != null) {
                SceneState.instance.onValueChange -= StateCallback;
            }
        }

        void StateCallback(bool aGlobal, string aName, SceneState.StateValue newVal) {
            if(global == aGlobal && name.Value == aName) {
                if(!iValueOut.IsNone)
                    iValueOut = newVal.ival;

                if(!fValueOut.IsNone)
                    fValueOut = newVal.fval;

                if(!FsmEvent.IsNullOrEmpty(changeEvent))
                    Fsm.Event(changeEvent);

                Finish();
            }
        }
    }
}

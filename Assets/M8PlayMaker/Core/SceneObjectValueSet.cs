using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer. Set the value to variable as either float or integer.  Only one or the other, not both!")]
    public class SceneObjectValueSet : FSMActionComponentBase<SceneSerializer> {
        [RequiredField]
        public FsmString name;

        public FsmInt iVal;
        public FsmInt fVal;

        public FsmBool persistent;

        public override void Reset() {
            base.Reset();

            name = null;
            iVal = null;
            fVal = null;
            persistent = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            if(SceneState.instance != null) {
                if(!iVal.IsNone) {
                    mComp.SetValue(name.Value, iVal.Value, persistent.Value);
                }
                else if(!fVal.IsNone) {
                    mComp.SetValueFloat(name.Value, fVal.Value, persistent.Value);
                }
            }

            Finish();
        }

        public override string ErrorCheck() {
            if(iVal.IsNone && fVal.IsNone)
                return "Either iVal or fVal needs to have a value.";
            return "";
        }
    }
}

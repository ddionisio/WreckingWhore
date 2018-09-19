using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("Set the value to variable as either float or integer.  Only one or the other, not both!")]
    public class SceneValueSet : FsmStateAction {
        [RequiredField]
        public FsmString name;

        public bool global;
                
        public FsmInt iVal;
        public FsmInt fVal;

        public FsmBool persistent;

        public override void Reset() {
            name = null;
            global = false;
            iVal = null;
            fVal = null;
            persistent = false;
        }

        public override void OnEnter() {
            if(SceneState.instance != null) {
                if(!iVal.IsNone) {
                    if(global)
                        SceneState.instance.SetGlobalValue(name.Value, iVal.Value, persistent.Value);
                    else
                        SceneState.instance.SetValue(name.Value, iVal.Value, persistent.Value);
                }
                else if(!fVal.IsNone) {
                    if(global)
                        SceneState.instance.SetGlobalValueFloat(name.Value, fVal.Value, persistent.Value);
                    else
                        SceneState.instance.SetValueFloat(name.Value, fVal.Value, persistent.Value);
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

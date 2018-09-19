using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker.TransformLib {
    [ActionCategory("Mate Transform")]
    [HutongGames.PlayMaker.Tooltip("Check if we are close to given target based on radius.")]
    public class IsNear : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject source.")]
        public FsmOwnerDefault gameObject;

        [HutongGames.PlayMaker.Tooltip("The GameObject to check against.")]
        public FsmGameObject targetObject;

        [HutongGames.PlayMaker.Tooltip("World position to check, or local offset from Target Object if specified.")]
        public FsmVector3 targetPosition;

        public FsmFloat targetRadius;

        [UIHint(UIHint.Variable)]
        public FsmBool result;

        public FsmEvent isTrue;

        public FsmEvent isFalse;

        [Title("Draw Debug Radius")]
        [HutongGames.PlayMaker.Tooltip("Draw a debug sphere for the approx. radius.")]
        public FsmBool debug;

        [HutongGames.PlayMaker.Tooltip("Color to use for the debug sphere.")]
        public FsmColor debugColor;

        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        public override void Reset() {
            gameObject = null;
            targetObject = null;
            targetPosition = new FsmVector3 { UseVariable = true };
            targetRadius = null;

            result = null;

            isTrue = null;
            isFalse = null;

            debug = false;
            debugColor = Color.yellow;

            everyFrame = false;
        }

        public override void OnEnter() {
            DoCheck();

            if(!everyFrame) {
                Finish();
            }
        }

        public override void OnLateUpdate() {
            DoCheck();
        }

        void DoCheck() {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }


            Vector3 targetPos = GetTargetPos();
            float targetR = GetTargetRadius();

            Transform t = go.transform;

            Vector3 delta = targetPos - t.position;

            bool isNear = delta.sqrMagnitude <= targetR * targetR;

            if(!result.IsNone)
                result.Value = isNear;

            if(isNear) {
                if(!FsmEvent.IsNullOrEmpty(isTrue))
                    Fsm.Event(isTrue);
            }
            else {
                if(!FsmEvent.IsNullOrEmpty(isFalse))
                    Fsm.Event(isFalse);
            }
        }

        Vector3 GetTargetPos() {
            var goTarget = targetObject.Value;
            Vector3 targetPos;
            if(goTarget != null) {
                targetPos = !targetPosition.IsNone ? goTarget.transform.TransformPoint(targetPosition.Value) : goTarget.transform.position;
            }
            else {
                targetPos = targetPosition.Value;
            }

            return targetPos;
        }

        float GetTargetRadius() {
            return targetRadius.IsNone ? 0.0f : targetRadius.Value;
        }

        public override void OnDrawGizmos() {
            if(debug.Value) {
                float r = GetTargetRadius();
                if(r > 0.0f) {
                    Gizmos.color = debugColor.Value;
                    Gizmos.DrawWireSphere(GetTargetPos(), r);
                }
            }
        }
    }

}

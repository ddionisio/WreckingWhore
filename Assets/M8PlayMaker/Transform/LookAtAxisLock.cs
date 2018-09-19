using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker.TransformLib {
    [ActionCategory("Mate Transform")]
    [HutongGames.PlayMaker.Tooltip("Set the forward vector of game object to target with restriction to axis. For now, it is only the up vector.")]
    public class LookAtAxisLock : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to rorate.")]
        public FsmOwnerDefault gameObject;

        [HutongGames.PlayMaker.Tooltip("The GameObject to Look At.")]
        public FsmGameObject targetObject;

        [HutongGames.PlayMaker.Tooltip("World position to look at, or local offset from Target Object if specified.")]
        public FsmVector3 targetPosition;

        [HutongGames.PlayMaker.Tooltip("Direction of the forward vector.")]
        public FsmBool backwards;

        [Title("Draw Debug Line")]
        [HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
        public FsmBool debug;

        [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;

        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        public override void Reset() {
            gameObject = null;
            targetObject = null;
            targetPosition = new FsmVector3 { UseVariable = true };
            backwards = false;

            debug = false;
            debugLineColor = Color.yellow;
            everyFrame = true;
        }

        public override void OnEnter() {
            DoLookAt();

            if(!everyFrame) {
                Finish();
            }
        }

        public override void OnLateUpdate() {
            DoLookAt();
        }

        void DoLookAt() {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            var goTarget = targetObject.Value;
            if(goTarget == null && targetPosition.IsNone) {
                return;
            }

            Vector3 lookAtPos;
            if(goTarget != null) {
                lookAtPos = !targetPosition.IsNone ? goTarget.transform.TransformPoint(targetPosition.Value) : goTarget.transform.position;
            }
            else {
                lookAtPos = targetPosition.Value;
            }

            Transform t = go.transform;
            float angle = M8.MathUtil.AngleForwardAxis(t.worldToLocalMatrix, t.position, backwards.Value ? Vector3.back : Vector3.forward, lookAtPos);
            t.rotation *= Quaternion.AngleAxis(angle, Vector3.up);

            if(debug.Value) {
                Debug.DrawLine(go.transform.position, lookAtPos, debugLineColor.Value);
            }
        }
    }
}

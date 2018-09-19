using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker.TransformLib {
    [ActionCategory("Mate Transform")]
    [HutongGames.PlayMaker.Tooltip("Set the up vector of game object to target")]
    public class UpLookAt : FsmStateAction {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to rorate.")]
        public FsmOwnerDefault gameObject;

        [HutongGames.PlayMaker.Tooltip("The GameObject to Look At.")]
        public FsmGameObject targetObject;

        [HutongGames.PlayMaker.Tooltip("World position to look at, or local offset from Target Object if specified.")]
        public FsmVector3 targetPosition;
        
        [HutongGames.PlayMaker.Tooltip("Don't rotate vertically.")]
        public FsmBool lockX;
        public FsmBool lockY;
        public FsmBool lockZ;

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

            lockX = false;
            lockY = false;
            lockZ = false;

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

            if(lockX.Value) {
                lookAtPos.x = go.transform.position.x;
            }

            if(lockY.Value) {
                lookAtPos.y = go.transform.position.y;
            }

            if(lockZ.Value) {
                lookAtPos.z = go.transform.position.z;
            }

            go.transform.up = lookAtPos - go.transform.position;

            if(debug.Value) {
                Debug.DrawLine(go.transform.position, lookAtPos, debugLineColor.Value);
            }
        }

    }
}
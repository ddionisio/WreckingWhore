using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Pool")]
    [HutongGames.PlayMaker.Tooltip("Check if given game object is released")]
    public class PoolReleaseCheck : FsmStateAction {
        public FsmString group;

        [RequiredField]
        public FsmGameObject gameObject;

        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public FsmEvent isTrue;
        public FsmEvent isFalse;

        public FsmBool everyFrame;

        public override void Reset() {
            group = null;
            gameObject = null;
            storeResult = null;
            isTrue = null;
            isFalse = null;
            everyFrame = null;
        }

        public override void OnEnter() {
            DoCheck();
            if(!everyFrame.Value)
                Finish();
        }

        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            bool isReleased;

#if POOLMANAGER
            isReleased = !PoolManager.Pools[group.Value].IsSpawned(gameObject.Value.transform);
#else
            PoolDataController pdc = gameObject.Value.GetComponent<PoolDataController>();
            isReleased = pdc != null ? pdc.claimed : false;
#endif

            if(!storeResult.IsNone)
                storeResult.Value = isReleased;

            if(isReleased)
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
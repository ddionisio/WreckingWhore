using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Pool")]
    [HutongGames.PlayMaker.Tooltip("Release given game object. If group is None, PoolController will determine what group this game object belongs to.")]
    public class PoolRelease : FsmStateAction {
        public FsmString group;

        [RequiredField]
        public FsmGameObject gameObject;

        public override void Reset() {
            base.Reset();

            group = null;
            gameObject = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
#if POOLMANAGER
            Debug.LogError("Not implemented!");
#else
            if(!group.IsNone)
                PoolController.ReleaseByGroup(group.Value, gameObject.Value.transform);
            else
                PoolController.ReleaseAuto(gameObject.Value.transform);
#endif
            Finish();
        }


    }
}
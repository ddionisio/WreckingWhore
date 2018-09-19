using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Pool")]
    [HutongGames.PlayMaker.Tooltip("Spawn a game object from pool")]
    public class PoolSpawn : FsmStateAction {
        [RequiredField]
        public FsmString group;

        [RequiredField]
        public FsmString type;

        public FsmString name;

        public FsmGameObject parent;

        public FsmVector3 position;

        [UIHint(UIHint.Variable)]
        public FsmGameObject toGameObject;

        public override void Reset() {
            base.Reset();

            group = null;
            type = null;
            name = null;

            parent = null;
            position = null;
            toGameObject = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
#if POOLMANAGER
            Debug.LogError("Not implemented!");
#else
            Transform t = PoolController.Spawn(group.Value, type.Value, name.IsNone ? type.Value : name.Value, parent.Value == null ? null : parent.Value.transform, null);
#endif
            if(t != null) {
                if(!toGameObject.IsNone)
                    toGameObject.Value = t.gameObject;
                else if(!position.IsNone) {
                    t.position = position.Value;
                }
            }
            else {
                Debug.LogWarning("Unable to spawn: " + type.Value + " group: " + group.Value);
            }

            Finish();
        }
    }
}
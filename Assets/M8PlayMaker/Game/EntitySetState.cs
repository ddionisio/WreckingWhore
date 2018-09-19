using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Entity")]
    [HutongGames.PlayMaker.Tooltip("Set entity state.")]
    public class EntitySetState : FSMActionComponentBase<EntityBase> {
        [RequiredField]
        public FsmInt state;

        public override void Reset() {
            base.Reset();

            state = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            mComp.state = state.Value;

            Finish();
        }
    }
}
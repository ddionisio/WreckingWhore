using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Entity")]
    [HutongGames.PlayMaker.Tooltip("Get entity's previous state.")]
    public class EntityGetStatePrev : FSMActionComponentBase<EntityBase> {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public override void Reset() {
            base.Reset();

            output = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            output.Value = mComp.prevState;

            Finish();
        }
    }
}
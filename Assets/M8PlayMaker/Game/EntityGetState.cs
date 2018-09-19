using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Entity")]
    [HutongGames.PlayMaker.Tooltip("Get entity's current state.")]
    public class EntityGetState : FSMActionComponentBase<EntityBase> {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public bool everyFrame = false;

        public override void Reset() {
            base.Reset();

            output = null;
            everyFrame = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            output.Value = mComp.state;

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            output.Value = mComp.state;
        }
    }
}
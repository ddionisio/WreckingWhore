using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Entity")]
    [HutongGames.PlayMaker.Tooltip("Removes entity and returns it to the manager")]
    public class EntityRelease : FSMActionComponentBase<EntityBase> {
        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            if(mComp != null)
                mComp.Release();

            Finish();
        }
    }
}
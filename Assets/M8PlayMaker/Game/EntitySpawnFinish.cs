using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Entity")]
    [HutongGames.PlayMaker.Tooltip("Let entity know that we have finished spawning.")]
    public class EntitySpawnFinish : FSMActionComponentBase<EntityBase> {
        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            if(mComp != null)
                mComp.SpawnFinish();

            Finish();
        }
    }
}
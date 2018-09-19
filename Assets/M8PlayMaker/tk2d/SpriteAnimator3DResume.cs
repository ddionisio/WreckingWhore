using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Resume the animator.")]
    public class SpriteAnimator3DResume : FSMActionComponentBase<SpriteAnimator3D> {

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Resume();

            Finish();
        }

    }
}

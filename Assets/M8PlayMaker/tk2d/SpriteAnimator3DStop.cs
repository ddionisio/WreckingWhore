using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Stop the animator.")]
    public class SpriteAnimator3DStop : FSMActionComponentBase<SpriteAnimator3D> {

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Stop();

            Finish();
        }

    }
}

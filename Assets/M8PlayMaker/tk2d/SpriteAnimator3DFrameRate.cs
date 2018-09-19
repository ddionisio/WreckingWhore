using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Set the framerate of the animator.")]
    public class SpriteAnimator3DFrameRate : FSMActionComponentBase<SpriteAnimator3D> {
        [HutongGames.PlayMaker.Tooltip("The frame per seconds of the current clip. Set this to None to reset the fps to default.")]
        public FsmFloat framePerSeconds;

        [HutongGames.PlayMaker.Tooltip("Repeat every Frame")]
        public bool everyFrame;

        public override void Reset() {
            base.Reset();

            framePerSeconds = null;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            DoIt();

            if(!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoIt();
        }

        void DoIt() {
            mComp.fps = framePerSeconds.IsNone ? tk2dSpriteAnimator.DefaultFps : framePerSeconds.Value;
        }
    }
}

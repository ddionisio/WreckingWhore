using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Play a clip.")]
    public class SpriteAnimator3DPlay : FSMActionComponentBase<SpriteAnimator3D> {

        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The clip name to play")]
        public FsmString clipName;

        public override void Reset() {
            base.Reset();

            clipName = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Play(clipName.Value);

            Finish();
        }

    }
}

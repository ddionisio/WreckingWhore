using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Check if the sprite 3d animator is playing.")]
    public class SpriteAnimator3DIsPlaying : FSMActionComponentBase<SpriteAnimator3D> {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The clip name to play")]
        public FsmString clipName;

        [HutongGames.PlayMaker.Tooltip("is the clip playing?")]
        [UIHint(UIHint.Variable)]
        public FsmBool isPlaying;

        [HutongGames.PlayMaker.Tooltip("EVvnt sent if clip is playing")]
        public FsmEvent isPlayingEvent;

        [HutongGames.PlayMaker.Tooltip("Event sent if clip is not playing")]
        public FsmEvent isNotPlayingEvent;

        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyframe;

        public override void Reset() {
            base.Reset();

            clipName = null;
            isPlaying = null;
            isPlayingEvent = null;
            isNotPlayingEvent = null;
            everyframe = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            DoCheck();

            if(!everyframe)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate() {
            DoCheck();
        }

        void DoCheck() {
            bool playing = mComp.IsPlaying(clipName.Value);

            if(!isPlaying.IsNone)
                isPlaying.Value = playing;

            if(playing)
                Fsm.Event(isPlayingEvent);
            else
                Fsm.Event(isNotPlayingEvent);
        }
    }
}

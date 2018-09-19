using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate tk2D")]
    [HutongGames.PlayMaker.Tooltip("Play a clip and wait for specific event.")]
    public class SpriteAnimator3DPlayWithEvents : FSMActionComponentBase<SpriteAnimator3D> {

        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The clip name to play")]
        public FsmString clipName;

        [HutongGames.PlayMaker.Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
        public FsmEvent animationTriggerEvent;

        [HutongGames.PlayMaker.Tooltip("Animation complete event. The event holds the clipId reference")]
        public FsmEvent animationCompleteEvent;

        public override void Reset() {
            base.Reset();

            clipName = null;
            animationTriggerEvent = null;
            animationCompleteEvent = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.Play(clipName.Value);

            if(FsmEvent.IsNullOrEmpty(animationTriggerEvent) && FsmEvent.IsNullOrEmpty(animationCompleteEvent))
                Finish();
            else {
                if(!FsmEvent.IsNullOrEmpty(animationTriggerEvent))
                    mComp.animationEventTriggeredCallback += OnTrigger;

                if(!FsmEvent.IsNullOrEmpty(animationCompleteEvent))
                    mComp.animationCompletedCallback += OnComplete;
            }
        }

        public override void OnExit() {
            mComp.animationCompletedCallback -= OnComplete;
            mComp.animationEventTriggeredCallback -= OnTrigger;

            base.OnExit();
        }

        void OnComplete(SpriteAnimator3D sprite) {
            Fsm.EventData.IntData = sprite.curClipIndex;
            Fsm.EventData.StringData = sprite.curClipName;
            Fsm.Event(animationCompleteEvent);
        }

        void OnTrigger(SpriteAnimator3D sprite, int frame) {
            tk2dSpriteAnimationFrame frameInfo = sprite.curClip.GetFrame(sprite.anim.CurrentFrame);

            Fsm.EventData.IntData = frameInfo.eventInt;
            Fsm.EventData.StringData = frameInfo.eventInfo;
            Fsm.EventData.FloatData = frameInfo.eventFloat;
            Fsm.Event(animationTriggerEvent);
        }
    }
}

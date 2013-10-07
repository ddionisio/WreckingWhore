// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions {
    [ActionCategory("2D Toolkit/SpriteAnimator")]
    [Tooltip("Plays a sprite animation. \nCan receive animation events and animation complete event. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
    public class Tk2dPlayAnimationWithEvents : FsmStateAction {
        [RequiredField]
        [Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
        [CheckForComponent(typeof(tk2dSpriteAnimator))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The clip name to play")]
        public FsmString clipName;

        [Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
        public FsmEvent animationTriggerEvent;

        [Tooltip("Animation complete event. The event holds the clipId reference")]
        public FsmEvent animationCompleteEvent;


        private tk2dSpriteAnimator _sprite;
        private tk2dSpriteAnimationClip _clip;

        private void _getSprite() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            _sprite = go.GetComponent<tk2dSpriteAnimator>();
        }


        public override void Reset() {
            gameObject = null;
            clipName = null;
            animationTriggerEvent = null;
            animationCompleteEvent = null;
        }

        public override void OnEnter() {
            _getSprite();

            DoPlayAnimationWithEvents();
        }

        void DoPlayAnimationWithEvents() {
            if(_sprite == null) {
                LogWarning("Missing tk2dSpriteAnimator component");
                return;
            }

            _clip = _sprite.GetClipByName(clipName.Value);

            if(_sprite.Playing == false) {
                _sprite.Play(_clip);
            }

            if(animationTriggerEvent != null) {
                _sprite.AnimationEventTriggered = AnimationEventDelegate;
            }
            if(animationCompleteEvent != null) {
                _sprite.AnimationCompleted = AnimationCompleteDelegate;
            }
        }

        void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum) {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);

            Fsm.EventData.IntData = frame.eventInt;
            Fsm.EventData.StringData = frame.eventInfo;
            Fsm.EventData.FloatData = frame.eventFloat;
            Fsm.Event(animationTriggerEvent);
        }

        void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
            if(_clip == clip) {
                //Fsm.EventData.IntData = _clip;
                Fsm.Event(animationCompleteEvent);
            }
        }
    }

}
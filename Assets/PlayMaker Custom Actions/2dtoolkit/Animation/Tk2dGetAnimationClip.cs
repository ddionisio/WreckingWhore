
using UnityEngine;

namespace HutongGames.PlayMaker.Actions {
    [ActionCategory("2D Toolkit/SpriteAnimator")]
    [Tooltip("Get the clip.")]
    public class Tk2dGetAnimationClip : FsmStateAction {
        [RequiredField]
        [Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
        [CheckForComponent(typeof(tk2dSpriteAnimator))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The clip name to play")]
        public FsmString output;

        [Tooltip("Get the default clip.")]
        public bool useDefault;

        [Tooltip("Repeat every frame.")]
        public bool everyframe;

        private tk2dSpriteAnimator _sprite;

        private void _getSprite() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(go == null) {
                return;
            }

            _sprite = go.GetComponent<tk2dSpriteAnimator>();
        }


        public override void Reset() {
            gameObject = null;
            output = null;
            useDefault = false;
            everyframe = false;

        }

        public override void OnEnter() {
            _getSprite();

            DoGetClip();

            if(!everyframe) {
                Finish();
            }

        }
        public override void OnUpdate() {
            DoGetClip();
        }


        void DoGetClip() {

            if(_sprite == null) {
                LogWarning("Missing tk2dSpriteAnimator component: " + _sprite.gameObject.name);
                return;
            }

            //_sprite.CurrentClip
            if(useDefault) {
                output.Value = _sprite.DefaultClip.name;
            }
            else if(_sprite.CurrentClip != null) {
                output.Value = _sprite.CurrentClip.name;
            }
        }

    }
}
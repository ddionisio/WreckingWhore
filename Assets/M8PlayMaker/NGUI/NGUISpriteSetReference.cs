using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate NGUI")]
    [HutongGames.PlayMaker.Tooltip("Set the sprite reference of a given widget.")]
    public class NGUISpriteSetReference : FSMActionComponentBase<UISprite> {
        [RequiredField]
        public FsmString sprite;

        public override void Reset() {
            base.Reset();

            sprite = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            base.OnEnter();

            mComp.spriteName = sprite.Value;
            mComp.MakePixelPerfect();

            Finish();
        }

    }
}

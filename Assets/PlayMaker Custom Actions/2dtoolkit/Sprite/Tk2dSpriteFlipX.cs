// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/Sprite")]
    [Tooltip("Flip X. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
    public class Tk2dSpriteFlipX : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
        [CheckForComponent(typeof(tk2dBaseSprite))]
        public FsmOwnerDefault gameObject;

        private tk2dBaseSprite _sprite;
        
        private void _getSprite()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) 
            {
                return;
            }
            
            _sprite =  go.GetComponent<tk2dBaseSprite>();
        }
        
        
        public override void Reset()
        {
            gameObject = null;
        }
        
        public override void OnEnter()
        {
            _getSprite();
            if(_sprite != null) {
                _sprite.FlipX = !_sprite.FlipX;
            }
            else
            {
                LogWarning("Missing tk2dBaseSprite component");
            }
            Finish();
        }
    }
}
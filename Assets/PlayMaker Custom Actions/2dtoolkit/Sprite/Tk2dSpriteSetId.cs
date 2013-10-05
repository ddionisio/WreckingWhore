// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the sprite id of a sprite. Can use id or name. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetId : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;
		
		[Tooltip("OR The sprite name ")]
		[UIHint(UIHint.FsmString)]
		public FsmString ORSpriteName;
		
		
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
			spriteID = null;
			ORSpriteName = null;
		}
		
		public override void OnEnter()
		{
			_getSprite();
			
			DoSetSpriteID();
			
			Finish();
		}

		void DoSetSpriteID()
		{

			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component: " + _sprite.gameObject.name);
				return;
			}
			
			
			int id = spriteID.Value;
					
			if (ORSpriteName.Value != "")
			{
				id = _sprite.GetSpriteIdByName(ORSpriteName.Value);
			}
		
			if (id!=_sprite.spriteId)
			{
				_sprite.spriteId = id;
			}
		}
		
	
	}
}
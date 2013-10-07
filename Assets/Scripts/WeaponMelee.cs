using UnityEngine;
using System.Collections;

public class WeaponMelee : Weapon {
    public tk2dSpriteAnimator anim;

    public string clip;

    private tk2dSpriteAnimationClip mClip;

    public override void Attack(bool isLeft) {
        base.Attack(isLeft);

        anim.Play(mClip);
    }

    public override void Finish() {

        base.Finish();
    }

    void Awake() {
        mClip = anim.GetClipByName(clip);

        anim.AnimationCompleted += OnAnimComplete;
    }

    void OnAnimComplete(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c) {
        if(c == mClip) {
            Finish();
        }
    }
}

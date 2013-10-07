using UnityEngine;
using System.Collections;

public class WeaponWhip : Weapon {
    public Transform line;
    public Transform ball;
    public Transform attach;
    public float length;
    public float delay;
    public AnimationCurve curve;

    public tk2dSpriteAnimator anim;

    public string clip;

    private tk2dSpriteAnimationClip mClip;
    private bool mIsLeft;

    public override void Attack(bool isLeft) {
        base.Attack(isLeft);

        mIsLeft = isLeft;

        anim.Play(mClip);
    }

    public override void Finish() {
        line.gameObject.SetActive(false);
        ball.gameObject.SetActive(false);

        base.Finish();
    }

    void Awake() {
        mClip = anim.GetClipByName(clip);

        anim.AnimationCompleted += OnAnimComplete;

        line.gameObject.SetActive(false);
        ball.gameObject.SetActive(false);
    }

    void OnAnimComplete(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c) {
        if(c == mClip) {
            StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack() {
        line.gameObject.SetActive(true);
        ball.gameObject.SetActive(true);

        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        float t = 0.0f;

        Vector3 lp = Vector3.zero;
        float lineZ = line.position.z;
        float ballZ = ball.position.z;

        while(mActive && t < delay) {
            Vector3 pos = attach.position;
            Quaternion r = attach.rotation;

            line.position = new Vector3(pos.x, pos.y, lineZ);
            line.rotation = r;

            float s = curve.Evaluate(t / delay);

            t += Time.fixedDeltaTime;

            float len = mIsLeft ? -s * length : s * length;

            Vector3 scale = line.localScale;
            scale.x = len;
            line.localScale = scale;

            lp.x = len;
            lp = r * lp;

            Vector3 ballPos = pos + lp;
            ball.position = new Vector3(ballPos.x, ballPos.y, ballZ);

            yield return wait;
        }

        Finish();
    }
}

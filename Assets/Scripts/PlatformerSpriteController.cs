using UnityEngine;
using System.Collections;

public class PlatformerSpriteController : MonoBehaviour {
    public delegate void Callback(PlatformerSpriteController ctrl);

    public tk2dSpriteAnimator anim;
    public PlatformerController controller;

    public string idleClip = "idle";
    public string moveClip = "move";

    public string[] upClips = { "up" }; //based on jump counter
    public string[] downClips = { "down" }; //based on jump counter

    public string wallStickClip = "wall";
    public string wallJumpClip = "wallJump";

    public ParticleSystem wallStickParticle;

    public event Callback flipCallback;

    //TODO: queue system

    private tk2dSpriteAnimationClip mIdle;
    private tk2dSpriteAnimationClip mMove;
    private tk2dSpriteAnimationClip[] mUps;
    private tk2dSpriteAnimationClip[] mDowns;
    private tk2dSpriteAnimationClip mWallStick;
    private tk2dSpriteAnimationClip mWallJump;

    private bool mIsLeft;
    private bool mAnimationActive = true;

    public bool isLeft { get { return mIsLeft; } }
    public bool animationActive { get { return mAnimationActive; } set { mAnimationActive = value; } }

    public void ResetAnimation() {
        mAnimationActive = true;
        mIsLeft = false;
        if(anim && anim.Sprite)
            anim.Sprite.FlipX = false;

        if(wallStickParticle) {
            wallStickParticle.loop = false;
            wallStickParticle.Stop();
        }
    }

    void OnDestroy() {
        flipCallback = null;
    }

    void Awake() {
        if(anim == null)
            anim = GetComponent<tk2dSpriteAnimator>();

        mIdle = anim.GetClipByName(idleClip);

        mMove = anim.GetClipByName(moveClip);

        mUps = new tk2dSpriteAnimationClip[upClips.Length];
        for(int i = 0, len = upClips.Length; i < len; i++)
            mUps[i] = anim.GetClipByName(upClips[i]);

        mDowns = new tk2dSpriteAnimationClip[downClips.Length];
        for(int i = 0, len = downClips.Length; i < len; i++)
            mDowns[i] = anim.GetClipByName(downClips[i]);

        mWallStick = anim.GetClipByName(wallStickClip);
        mWallJump = anim.GetClipByName(wallJumpClip);

        if(controller == null)
            controller = GetComponent<PlatformerController>();
    }

    tk2dSpriteAnimationClip GetMidAirClip(tk2dSpriteAnimationClip[] clips) {
        if(clips == null || clips.Length == 0)
            return null;

        int ind = controller.jumpCounterCurrent;

        return ind >= clips.Length ? clips[clips.Length - 1] : clips[ind];
    }

    // Update is called once per frame
    void Update() {
        if(mAnimationActive) {
            bool left = mIsLeft;

            if(controller.isJumpWall) {
                anim.Play(mWallJump);

                left = controller.localVelocity.x < 0.0f;
            }
            else if(controller.isWallStick) {
                if(wallStickParticle) {
                    if(wallStickParticle.isStopped) {
                        wallStickParticle.Play();
                    }

                    wallStickParticle.loop = true;
                }

                anim.Play(mWallStick);

                left = M8.MathUtil.CheckSide(controller.wallStickCollide.normal, controller.dirHolder.up) == M8.MathUtil.Side.Right;

            }
            else {
                if(wallStickParticle)
                    wallStickParticle.loop = false;

                if(controller.isGrounded) {
                    if(controller.moveSide != 0.0f) {
                        anim.Play(mMove);
                    }
                    else {
                        anim.Play(mIdle);
                    }
                }
                else {
                    tk2dSpriteAnimationClip clip;

                    if(controller.localVelocity.y <= 0.0f) {
                        clip = GetMidAirClip(mDowns);
                    }
                    else {
                        clip = GetMidAirClip(mUps);
                    }

                    if(clip != null)
                        anim.Play(clip);
                }

                if(controller.moveSide != 0.0f) {
                    left = controller.moveSide < 0.0f;
                }
            }

            if(mIsLeft != left) {
                mIsLeft = left;

                anim.Sprite.FlipX = mIsLeft;

                if(flipCallback != null)
                    flipCallback(this);
            }
        }
    }
}

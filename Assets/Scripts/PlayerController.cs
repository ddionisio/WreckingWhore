using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public const int playerIndex = 0;

    public WeaponMelee hammer;
    public WeaponWhip whip;

    public float knockbackForce = 40.0f;
    public float knockbackDelay = 0.2f;
    public float knockbackAngle = 30.0f;

    private Player mPlayer;
    private PlatformerController mPlatformer;
    private PlatformerSpriteController mPlatformerSpriteCtrl;

    private bool mInputEnabled;
    private bool mKnockbackActive;
    private float mKnockbackCurTime;
    private Vector3 mKnockbackDir;

    public Player player { get { return mPlayer; } }
    public PlatformerController platformer { get { return mPlatformer; } }
    public PlatformerSpriteController platformerSpriteControl { get { return mPlatformerSpriteCtrl; } }

    public bool inputEnabled {
        get { return mInputEnabled; }

        set {
            if(mInputEnabled != value) {
                InputManager input = Main.instance ? Main.instance.input : null;

                if(input) {
                    mInputEnabled = value;

                    if(mInputEnabled) {
                        input.AddButtonCall(playerIndex, InputAction.Attack1, OnInputAttackPrimary);
                        input.AddButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
                        input.AddButtonCall(playerIndex, InputAction.Special, OnInputSpecial);
                    }
                    else {
                        input.RemoveButtonCall(playerIndex, InputAction.Attack1, OnInputAttackPrimary);
                        input.RemoveButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
                        input.RemoveButtonCall(playerIndex, InputAction.Special, OnInputSpecial);
                    }

                    mPlatformer.inputEnabled = mInputEnabled;
                }
            }
        }
    }

    void OnDestroy() {
        inputEnabled = false;
    }

    void Awake() {
        mPlatformer = GetComponent<PlatformerController>();
        mPlatformer.player = playerIndex;
        mPlatformer.moveInputX = InputAction.Horizontal;
        mPlatformer.moveInputY = InputAction.Vertical;
        mPlatformer.jumpInput = InputAction.Jump;

        mPlatformerSpriteCtrl = GetComponent<PlatformerSpriteController>();

        hammer.finishCallback += OnAttackFinish;
        whip.finishCallback += OnAttackFinish;
    }

    void EntityStart(EntityBase ent) {
        mPlayer = ent as Player;
        mPlayer.setStateCallback += OnEntitySetState;
    }

    // Update is called once per frame
    void Update() {
    }

    void OnEntitySetState(EntityBase ent) {
        switch((EntityState)ent.prevState) {
            case EntityState.Normal:
                inputEnabled = false;
                mPlatformerSpriteCtrl.animationActive = false;
                break;

            case EntityState.Hurt:
                mPlatformer.lockDrag = false;
                mPlatformer.ResetCollision();
                mKnockbackActive = false;
                break;

            case EntityState.Attack:
                if(hammer.isActive)
                    hammer.Cancel();

                if(whip.isActive)
                    whip.Cancel();
                break;

        }

        switch((EntityState)ent.state) {
            case EntityState.Normal:
                mPlatformerSpriteCtrl.animationActive = true;
                inputEnabled = true;
                break;

            case EntityState.Attack:
                break;

            case EntityState.Hurt:
                mPlatformer.lockDrag = true;
                mPlatformer.rigidbody.drag = 0.0f;
                mPlatformer.rigidbody.velocity = Vector3.zero;

                if(mKnockbackActive) {
                    CalculateKnockbackDir();
                    mKnockbackCurTime = 0.0f;
                }
                else
                    StartCoroutine(DoKnockback());

                mPlatformerSpriteCtrl.anim.Play("hurt");
                break;

            case EntityState.Dead:

                mPlatformerSpriteCtrl.anim.Play("dead");
                break;

            case EntityState.Invalid:
                mPlatformer.ResetCollision();
                break;
        }
    }

    void OnInputAttackPrimary(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            mPlayer.state = (int)EntityState.Attack;
            hammer.Attack(mPlatformerSpriteCtrl.isLeft);
        }
    }

    void OnInputAttackSecondary(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            mPlayer.state = (int)EntityState.Attack;
            whip.Attack(mPlatformerSpriteCtrl.isLeft);
        }
    }

    void OnInputSpecial(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
        }
    }

    void OnAttackFinish(Weapon weap) {
        if((EntityState)mPlayer.state == EntityState.Attack) {
            mPlayer.state = (int)EntityState.Normal;
        }
    }

    void CalculateKnockbackDir() {
        float d = mPlatformer.collider.bounds.center.x - mPlayer.lastHitPosition.x;
        if(d == 0)
            d = 1;

        Vector3 delta = new Vector3(Mathf.Sign(d), 0);
        mKnockbackDir = Quaternion.AngleAxis(delta.x < 0 ? -knockbackAngle : knockbackAngle, Vector3.forward) * delta;
    }

    IEnumerator DoKnockback() {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        mKnockbackActive = true;
        mKnockbackCurTime = 0.0f;

        CalculateKnockbackDir();

        while(mKnockbackActive && mKnockbackCurTime < knockbackDelay) {
            mPlatformer.rigidbody.AddForce(mKnockbackDir * knockbackForce);
            mKnockbackCurTime += Time.fixedDeltaTime;
            yield return wait;
        }

        mKnockbackActive = false;
    }
}

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public const int playerIndex = 0;

    public GameObject weaponHolder;

    public float knockbackForce = 40.0f;
    public float knockbackDelay = 0.2f;
    public float knockbackAngle = 30.0f;

    public bool startWeaponEnabled = true;

    private Player mPlayer;
    private PlatformerController mPlatformer;
    private PlatformerSpriteController mPlatformerSpriteCtrl;

    private bool mInputEnabled;
    private bool mKnockbackActive;
    private float mKnockbackCurTime;
    private Vector3 mKnockbackDir;
    private Weapon.Dir mWeaponLastDir = Weapon.Dir.Right;

    private Weapon[] mWeapons;

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
                        input.AddButtonCall(playerIndex, InputAction.Jump, OnInputJump);
                        //input.AddButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
                        input.AddButtonCall(playerIndex, InputAction.Special, OnInputSpecial);
                    }
                    else {
                        input.RemoveButtonCall(playerIndex, InputAction.Attack1, OnInputAttackPrimary);
                        input.RemoveButtonCall(playerIndex, InputAction.Jump, OnInputJump);
                        //input.RemoveButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
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

        mWeapons = weaponHolder.GetComponentsInChildren<Weapon>(true);

        weaponHolder.SetActive(false);
    }

    void EntityStart(EntityBase ent) {
        mPlayer = ent as Player;
        mPlayer.setStateCallback += OnEntitySetState;

        weaponHolder.SetActive(startWeaponEnabled);
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch((EntityState)mPlayer.state) {
            case EntityState.Normal:
            case EntityState.Attack:
                InputManager input = Main.instance.input;
                float xAxis = input.GetAxis(0, InputAction.Horizontal);
                float yAxis = input.GetAxis(0, InputAction.Vertical);

                if(yAxis < -0.01f || ((EntityState)mPlayer.state == EntityState.Attack && input.IsDown(0, InputAction.Jump))) {
                    mWeaponLastDir = Weapon.Dir.Down;
                    for(int i = 0, max = mWeapons.Length; i < max; i++)
                        mWeapons[i].SetDir(Weapon.Dir.Down);
                }
                else if(yAxis > 0.01f) {
                    mWeaponLastDir = Weapon.Dir.Up;
                    for(int i = 0, max = mWeapons.Length; i < max; i++)
                        mWeapons[i].SetDir(Weapon.Dir.Up);
                }
                else if(xAxis < -0.01f) {
                    mWeaponLastDir = Weapon.Dir.Left;
                    for(int i = 0, max = mWeapons.Length; i < max; i++)
                        mWeapons[i].SetDir(Weapon.Dir.Left);
                }
                else if(xAxis > 0.01f) {
                    mWeaponLastDir = Weapon.Dir.Right;
                    for(int i = 0, max = mWeapons.Length; i < max; i++)
                        mWeapons[i].SetDir(Weapon.Dir.Right);
                }
                else if(mWeaponLastDir == Weapon.Dir.Up || mWeaponLastDir == Weapon.Dir.Down) {
                    mWeaponLastDir = mPlatformerSpriteCtrl.isLeft ? Weapon.Dir.Left : Weapon.Dir.Right;
                    for(int i = 0, max = mWeapons.Length; i < max; i++)
                        mWeapons[i].SetDir(mWeaponLastDir);
                }
                break;
        }
    }

    void OnEntitySetState(EntityBase ent) {
        switch((EntityState)ent.prevState) {
            case EntityState.Normal:
                break;

            case EntityState.Hurt:
                mPlatformer.lockDrag = false;
                mPlatformer.ResetCollision();
                mKnockbackActive = false;
                break;

            case EntityState.Attack:
                foreach(Weapon weapon in mWeapons)
                    weapon.Cancel();
                break;

        }

        switch((EntityState)ent.state) {
            case EntityState.Normal:
                inputEnabled = true;

                mPlatformerSpriteCtrl.animationActive = true;
                mPlatformerSpriteCtrl.state = PlatformerSpriteController.State.None;
                break;

            case EntityState.Attack:
                inputEnabled = true;

                mPlatformerSpriteCtrl.animationActive = true;
                mPlatformerSpriteCtrl.state = PlatformerSpriteController.State.Attack;

                foreach(Weapon weapon in mWeapons)
                    weapon.Attack();
                break;

            case EntityState.Hurt:
                SoundPlayerGlobal.instance.Play("ouch");

                inputEnabled = false;

                mPlatformer.moveSide = 0.0f;
                mPlatformer.lockDrag = true;
                mPlatformer.rigidbody.drag = 0.0f;
                mPlatformer.rigidbody.velocity = Vector3.zero;
                mPlatformer.jumpCounterCurrent = mPlatformer.jumpCounter;

                if(mKnockbackActive) {
                    mKnockbackCurTime = 0.0f;
                }
                else
                    StartCoroutine(DoKnockback());

                mPlatformerSpriteCtrl.animationActive = false;
                mPlatformerSpriteCtrl.anim.Play("hurt");
                break;

            case EntityState.Dead:
                inputEnabled = false;

                mPlatformer.moveSide = 0.0f;
                mPlatformer.rigidbody.velocity = Vector3.zero;

                mPlatformerSpriteCtrl.animationActive = false;
                mPlatformerSpriteCtrl.anim.Play("dead");
                break;

            case EntityState.Invalid:
                inputEnabled = false;

                mPlatformer.ResetCollision();
                break;
        }
    }

    void OnInputAttackPrimary(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            if(weaponHolder.activeSelf && mPlayer.canAttack) {
                SoundPlayerGlobal.instance.Play("fire");

                mPlayer.state = (int)EntityState.Attack;
            }
        }
        else if(dat.state == InputManager.State.Released) {
            if(mPlayer.state == (int)EntityState.Attack)
                mPlayer.state = (int)EntityState.Normal;
        }

    }

    void OnInputJump(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            if(!mPlatformer.isGrounded && weaponHolder.activeSelf && mPlayer.canAttack) {
                for(int i = 0, max = mWeapons.Length; i < max; i++)
                    mWeapons[i].SetDir(Weapon.Dir.Down);

                SoundPlayerGlobal.instance.Play("fire");

                mPlayer.state = (int)EntityState.Attack;
            }
        }
        else if(dat.state == InputManager.State.Released) {
            InputManager input = Main.instance.input;
            if(mPlayer.state == (int)EntityState.Attack && !input.IsDown(0, InputAction.Attack1)) {
                mPlayer.state = (int)EntityState.Normal;
            }
        }
    }

    void OnInputAttackSecondary(InputManager.Info dat) {
        OnInputAttackPrimary(dat);

        if(dat.state == InputManager.State.Pressed) {
            for(int i = 0, max = mWeapons.Length; i < max; i++)
                mWeapons[i].SetDir(Weapon.Dir.Down);
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

    public void CalculateKnockbackDir(Vector3 lastHit) {
        float d = mPlatformer.collider.bounds.center.x - lastHit.x;
        if(d == 0)
            d = 1;

        Vector3 delta = new Vector3(Mathf.Sign(d), 0);
        mKnockbackDir = Quaternion.AngleAxis(delta.x < 0 ? -knockbackAngle : knockbackAngle, Vector3.forward) * delta;
    }

    public void SetKnockbackDir(Vector3 dir) {
        mKnockbackDir = dir;
    }

    IEnumerator DoKnockback() {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        mKnockbackActive = true;
        mKnockbackCurTime = 0.0f;

        while(mKnockbackActive && mKnockbackCurTime < knockbackDelay) {
            mPlatformer.rigidbody.AddForce(mKnockbackDir * knockbackForce);
            mKnockbackCurTime += Time.fixedDeltaTime;
            yield return wait;
        }

        mKnockbackActive = false;
    }

    void Update() {
        switch((EntityState)mPlayer.state) {
            case EntityState.Attack:
                float pow = mPlayer.powerAttackConsume * Time.deltaTime;

                if(mPlayer.stats.power < pow)
                    mPlayer.state = (int)EntityState.Normal;
                else
                    mPlayer.stats.power -= pow;
                break;
        }
    }
}

using UnityEngine;
using System.Collections;

public class Player : EntityBase {
    private static Player mInstance;

    private PlayerController mCtrl;
    private Stats mStats;
    private Vector3 mLastHitPosition;

    public float hurtDelay = 0.5f;
    public float hurtInvul = 1.0f;

    public LayerMask spikeCollisionMask;
    public float spikeDamage = 2.5f;

    public float powerAttackConsume = 1.0f;

    public SpriteColorBlink blinkSprite;

    private float mLastHurtTime;

    private bool mPauseInputEnabled;

    public static Player instance { get { return mInstance; } }

    public Stats stats { get { return mStats; } }
    public PlayerController controller { get { return mCtrl; } }

    public Vector3 lastHitPosition { get { return mLastHitPosition; } }

    public bool isInvulnerable {
        get {
            return Time.time - mLastHurtTime < hurtInvul;
        }
    }

    public bool canAttack {
        get {
            return ((EntityState)state == EntityState.Normal || (EntityState)state == EntityState.Attack) && mStats.power >= powerAttackConsume;
        }
    }

    public bool pauseInputEnabled {
        get { return mPauseInputEnabled; }
        set {
            if(mPauseInputEnabled != value) {
                mPauseInputEnabled = value;

                InputManager input = Main.instance ? Main.instance.input : null;
                if(input) {
                    if(mPauseInputEnabled) {
                        input.AddButtonCall(0, InputAction.Menu, OnInputPause);
                    }
                    else {
                        input.RemoveButtonCall(0, InputAction.Menu, OnInputPause);
                    }
                }
            }
        }
    }

    protected override void StateChanged() {
        switch((EntityState)state) {
            case EntityState.Normal:
                mStats.powerRegenActive = true;
                break;

            case EntityState.Attack:
                mStats.powerRegenActive = false;
                break;

            case EntityState.Hurt:
                Blink(hurtDelay);
                mLastHurtTime = Time.time;
                break;

            case EntityState.Dead:
                Debug.Log("dead");
                break;
        }
    }

    protected override void SetBlink(bool blink) {
        if(blinkSprite)
            blinkSprite.enabled = blink;

        if(blink) {
        }
        else {
            switch((EntityState)state) {
                case EntityState.Hurt:
                    state = (int)EntityState.Normal;
                    break;
            }
        }
    }

    protected override void OnDespawned() {
        //reset stuff here
        mStats.Reset();

        state = (int)EntityState.Invalid;

        base.OnDespawned();
    }

    protected override void OnDestroy() {
        if(mInstance == this) {
            //dealloc here
            pauseInputEnabled = false;

            base.OnDestroy();
        }
    }

    public override void SpawnFinish() {
        //start ai, player control, etc
        mStats.Reset();
        state = (int)EntityState.Normal;
    }

    protected override void SpawnStart() {
        //initialize some things
    }

    protected override void Awake() {
        if(mInstance == null) {
            mInstance = this;

            base.Awake();

            //initialize variables
            mStats = GetComponent<Stats>();
            mStats.statCallback += OnStatChange;

            mCtrl = GetComponent<PlayerController>();

            if(blinkSprite)
                blinkSprite.enabled = false;
        }
        else
            DestroyImmediate(gameObject);
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)

        CameraBound cb = mCtrl.platformer.eye.GetComponentInChildren<CameraBound>();
        cb.bounds = LevelManager.instance.levelBounds;

        pauseInputEnabled = true;
    }

    void OnStatChange(Stats stat, Stats.Type which, float delta) {
        switch(which) {
            case Stats.Type.HP:
                if(stat.hp <= 0.0f)
                    state = (int)EntityState.Dead;
                else if(delta < 0) {
                    state = (int)EntityState.Hurt;
                }
                break;
        }
    }

    void OnHit(HitTrigger hit) {
        mLastHitPosition = hit.collider ? hit.collider.bounds.center : hit.transform.position;

        //check invul
        if((state == (int)EntityState.Normal || state == (int)EntityState.Attack) && !isInvulnerable) {
            mCtrl.CalculateKnockbackDir(mLastHitPosition);
            mStats.Damage(hit.damage);
        }
    }

    void OnProjectileHit(Projectile proj) {
        mLastHitPosition = proj.collider ? proj.collider.bounds.center : proj.transform.position;

        //check invul
        if((state == (int)EntityState.Normal || state == (int)EntityState.Attack) && !isInvulnerable) {
            mCtrl.CalculateKnockbackDir(mLastHitPosition);
            mStats.Damage(proj.damage);
        }
    }

    void OnCollisionStay(Collision col) {
        if(((1 << col.gameObject.layer) & spikeCollisionMask) != 0) {
            if((state == (int)EntityState.Normal || state == (int)EntityState.Attack) && !isInvulnerable) {
                mCtrl.SetKnockbackDir(col.contacts[0].normal);
                mStats.Damage(spikeDamage);
            }
        }
    }

    void OnInputPause(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            UIModalManager.instance.ModalOpen("pause");
        }
    }

    void OnUIModalActive() {
        Main.instance.sceneManager.Pause();

        pauseInputEnabled = false;
    }

    void OnUIModalInactive() {
        Main.instance.sceneManager.Resume();

        pauseInputEnabled = true;
    }
}

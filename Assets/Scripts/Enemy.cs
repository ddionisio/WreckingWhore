using UnityEngine;
using System.Collections;

public class Enemy : EntityBase {
    public enum BodySpriteState {
        idle,
        move,
    }

    public Collider bodyCollider;
    public bool facePlayer;
    public tk2dSpriteAnimator bodySpriteAnim;
    public tk2dBaseSprite bodySprite;
    public Transform mover;
    public SpriteColorBlink hurtBlink;

    public bool attackUseNormalUpdate = false;

    public EnemyShootController shoot;
    public float shootCooldown = 1.0f;

    public float bodyClipMoveThreshold = 0.015f;

    public float hurtDelay = 0.3f;

    public bool releaseOnSleep;
    public bool allowKnockback;
    public float knockbackImpulse = 4.0f;
    public float knockbackAngle = 30.0f;

    public HitTrigger bodyHit;

    private BodySpriteState mBodySpriteState = BodySpriteState.idle;

    private Transform mPlayerTrans;
    private Transform mBodyTrans;

    private tk2dSpriteAnimationClip[] mBodySpriteClips;
    private bool mNormalUpdateActive;

    private Stats mStats;

    public Stats stats { get { return mStats; } }

    public void SetCollisionActive(bool yes) {
        if(bodyCollider) {
            bodyCollider.enabled = yes;

            if(bodyCollider.rigidbody)
                bodyCollider.rigidbody.detectCollisions = yes;
        }
    }

    public BodySpriteState bodySpriteState {
        get { return mBodySpriteState; }
        set {
            //Debug.Log("f: " + value);

            mBodySpriteState = value;

            if(!Application.isPlaying || bodySpriteAnim == null)
                return;

            if(mBodySpriteClips[(int)mBodySpriteState] != null)
                bodySpriteAnim.Play(mBodySpriteClips[(int)mBodySpriteState]);
        }
    }

    void ApplyState() {
        switch((EntityState)state) {
            case EntityState.Attack:
                if(bodyHit)
                    bodyHit.gameObject.SetActive(true);

                //SetCollisionActive(true);

                if(attackUseNormalUpdate && !mNormalUpdateActive) {
                    StartCoroutine(DoNormalMoverUpdate());
                }
                break;

            case EntityState.Normal:
                if(bodyHit)
                    bodyHit.gameObject.SetActive(true);

                //SetCollisionActive(true);

                if(!mNormalUpdateActive)
                    StartCoroutine(DoNormalMoverUpdate());
                break;

            case EntityState.Hurt:
                if(bodyHit)
                    bodyHit.gameObject.SetActive(false);
                break;

            case EntityState.Dead:
                mStats.Reset();

                if(bodyHit)
                    bodyHit.gameObject.SetActive(false);
                //SetCollisionActive(false);
                //bodySpriteState = BodySpriteState.dead;
                //Debug.Log("dead");
                break;

            case EntityState.Invalid:
                if(hurtBlink)
                    hurtBlink.enabled = false;
                break;
        }
    }

    protected override void StateChanged() {
        switch((EntityState)prevState) {
            case EntityState.Normal:
                if(shoot)
                    shoot.shootEnable = false;
                break;
        }

        ApplyState();

        switch((EntityState)state) {
            case EntityState.Hurt:
                Blink(hurtDelay);
                break;

            case EntityState.Dead:
                if(shoot)
                    shoot.ClearProjectiles();
                break;
        }
    }

    protected override void SetBlink(bool blink) {
        if(hurtBlink)
            hurtBlink.enabled = blink;

        if(blink) {
        }
        else {
            if(state == (int)EntityState.Hurt)
                state = (int)EntityState.Normal;
        }
    }

    public override void Release() {
        state = StateInvalid;
        mNormalUpdateActive = false;

        if(mStats)
            mStats.Reset();

        base.Release();
    }

    protected override void OnDespawned() {
        //reset stuff here

        base.OnDespawned();
    }

    protected override void OnDestroy() {
        //dealloc here

        base.OnDestroy();
    }

    protected override void ActivatorWakeUp() {
        base.ActivatorWakeUp();

        ApplyState();
    }

    protected override void ActivatorSleep() {
        if(releaseOnSleep)
            Release();
        else {
            base.ActivatorSleep();

            mNormalUpdateActive = false;
        }
    }

    public override void SpawnFinish() {
        //start ai, player control, etc
        state = (int)EntityState.Normal;
    }

    protected override void SpawnStart() {
        //initialize some things
        mPlayerTrans = Player.instance.transform;
    }

    protected override void Awake() {
        base.Awake();

        //initialize variables
        mStats = GetComponent<Stats>();
        if(mStats)
            mStats.statCallback += OnStatChange;

        if(bodySpriteAnim != null) {
            mBodySpriteClips = M8.tk2dUtil.GetSpriteClips(bodySpriteAnim, typeof(BodySpriteState));
        }

        if(bodySprite == null && bodySpriteAnim != null)
            bodySprite = bodySpriteAnim.Sprite;

        if(bodyCollider)
            mBodyTrans = bodyCollider.transform;

        if(shoot)
            shoot.shootCallback += OnShoot;

        if(hurtBlink)
            hurtBlink.enabled = false;
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)
    }

    void OnShoot(EnemyShootController ctrl) {
        state = (int)EntityState.Attack;
    }

    IEnumerator DoNormalMoverUpdate() {
        mNormalUpdateActive = true;

        WaitForSeconds waitDelay = new WaitForSeconds(0.1f);

        Vector3 lastMoverPos = mover ? mover.position : transform.position;

        float lastShootTime = Time.fixedTime;

        bodySpriteState = BodySpriteState.idle;

        while((EntityState)state == EntityState.Normal || ((EntityState)state == EntityState.Attack && attackUseNormalUpdate)) {
            if(shoot && !shoot.shootEnable) {
                if(Time.fixedTime - lastShootTime >= shootCooldown) {
                    lastShootTime = Time.fixedTime;
                    shoot.shootEnable = true;
                }
            }

            Vector3 moverPos = mover ? mover.position : transform.position;
            Vector3 delta;

            if(lastMoverPos != moverPos) {
                delta = moverPos - lastMoverPos;

                delta = mBodyTrans.worldToLocalMatrix.MultiplyVector(delta);
            }
            else
                delta = Vector3.zero;

            lastMoverPos = moverPos;

            //determine animation
            if(bodySpriteAnim) {
                if(Mathf.Abs(delta.x) < bodyClipMoveThreshold) {
                    bodySpriteState = BodySpriteState.idle;
                }
                else {
                    bodySpriteState = BodySpriteState.move;
                }
            }

            //determine facing
            if(bodySprite) {
                if(facePlayer) {
                    Vector3 dir = mPlayerTrans.transform.position - mBodyTrans.position;
                    dir = mBodyTrans.worldToLocalMatrix.MultiplyVector(dir);

                    if(dir.x != 0.0f) {
                        bodySprite.FlipX = dir.x < 0.0f;

                        if(shoot)
                            shoot.visionDir.x = dir.x < 0.0f ? -Mathf.Abs(shoot.visionDir.x) : Mathf.Abs(shoot.visionDir.x);
                    }
                }
                else if(delta.x != 0.0f) {
                    bodySprite.FlipX = delta.x < 0.0f;

                    if(shoot)
                        shoot.visionDir.x = delta.x < 0.0f ? -Mathf.Abs(shoot.visionDir.x) : Mathf.Abs(shoot.visionDir.x);
                }
            }

            yield return waitDelay;
        }

        mNormalUpdateActive = false;
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

    /*void OnTriggerStay(Collider col) {
        if(col.GetComponent<HitTrigger>()) {
            Debug.Log("fuck");
        }
    }*/

    void OnHit(HitTrigger hit) {
        //Debug.Log("hit: " + hit.damage);
        switch((EntityState)state) {
            case EntityState.Normal:
            case EntityState.Attack:
                mStats.Damage(hit.damage);

                if(allowKnockback && hit.knockback && bodyCollider && !bodyCollider.rigidbody.isKinematic) {
                    Vector3 hitPos = hit.collider ? hit.collider.bounds.center : hit.transform.position;
                    Vector3 delta = new Vector3(Mathf.Sign(bodyCollider.bounds.center.x - hitPos.x), 0);
                    delta = Quaternion.AngleAxis(delta.x < 0 ? -knockbackAngle : knockbackAngle, Vector3.forward) * delta;
                    bodyCollider.rigidbody.AddForce(delta * knockbackImpulse, ForceMode.Impulse);
                }
                break;
        }
    }
}

using UnityEngine;
using System.Collections;

public class Boss : EntityBase {
    public const float dampMaxSpeed = Mathf.Infinity;
    public const string projGrp = "proj";
    public const string projType = "enemyProjFireball";
    public enum State {
        Invalid = -1,

        Revert,

        Pattern0, //do nothing
        Pattern1,
        Pattern2,
        Pattern3,
        Pattern4,

        DeathPrepare,
        Death,

        MaxPatternRandom = Pattern3 + 1
    }

    public AnimatorData bodyAnimator;

    public tk2dBaseSprite[] face;
    public tk2dBaseSprite[] hair;

    public Transform[] eyes;

    public Transform mouthPoint;

    public SpriteColorPulse[] hurtPulses;

    public GameObject tongue;

    public int maxFireballs = 5;

    public float hurtDelay = 1.0f;

    public float wait = 1.0f;

    public Color invulColor;

    public float revertDelay;

    public Transform pattern1FixedY;
    public float pattern1FollowDelay = 0.25f;
    public float pattern1ShootDelay = 1.0f;
    public float pattern1Delay = 6.0f;

    public Transform pattern2FixedY;
    public float pattern2FollowDelay = 1.0f;
    public AnimatorData pattern2TongueAnimator;
    public float pattern2TongueFollowDelay = 0.2f;
    public float pattern2TonguePlayDelayStart = 3.0f;
    public float pattern2TonguePlayDelay = 1.5f;
    public int pattern2TongueNumPlay = 3;

    public string pattern3EyesFadeTake = "eyesFade";
    public string pattern3EyesRevertTake = "eyesRevert";
    public GameObject pattern3EyesRotate;
    public float pattern3Accel = 10.0f;
    public float pattern3SpeedCap = 5.0f;
    public float pattern3Delay = 5.0f;

    public float deathRevertDelay = 2.0f;

    public string deathTake = "die";

    private Player mPlayer;

    private bool mEyeFollowPlayer;
    private bool mInvul;

    private int mCurNumFireballs;

    private float mCurTime;

    private Vector3 mOrigPos;

    private Stats mStats;

    private Vector3 mCurFollowVel;

    private float mPattern1FixedY;

    private float mPattern2FixedY;
    private int mPattern2CurNumPlay;
    private Vector3 mPattern2TongueCurVel;

    private Vector3 mDeathPrepStartPos;

    public Stats stats { get { return mStats; } }

    public bool invulnerable {
        get { return mInvul; }
        set {
            if(mInvul != value) {
                mInvul = value;
                if(mInvul) {
                    foreach(tk2dBaseSprite s in face)
                        s.color = invulColor;
                }
                else {
                    foreach(tk2dBaseSprite s in face)
                        s.color = Color.white;
                }
            }
        }
    }

    protected override void StateChanged() {
        CancelInvoke();

        mCurTime = 0.0f;
        tongue.SetActive(false);
        mCurFollowVel = Vector3.zero;

        mEyeFollowPlayer = true;
        invulnerable = false;

        if(!rigidbody.isKinematic) {
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }

        switch((State)state) {
            case State.Revert:
                invulnerable = true;
                break;

            case State.Pattern0:
                break;

            case State.Pattern1:
                tongue.SetActive(true);

                mCurNumFireballs = 0;

                InvokeRepeating("ShootFireballFromMouth", pattern1ShootDelay, pattern1ShootDelay);
                break;

            case State.Pattern2:
                invulnerable = true;

                pattern2TongueAnimator.gameObject.SetActive(true);
                mPattern2TongueCurVel = Vector3.zero;
                mPattern2CurNumPlay = 0;
                break;

            case State.Pattern3:
                mEyeFollowPlayer = false;
                foreach(Transform eye in eyes) {
                    eye.up = Vector3.up;
                }

                rigidbody.isKinematic = false;
                bodyAnimator.Play(pattern3EyesFadeTake);
                break;

            case State.DeathPrepare:
                mEyeFollowPlayer = false;

                foreach(Transform eye in eyes) {
                    eye.up = Vector3.up;
                }

                mDeathPrepStartPos = transform.position;
                break;

            case State.Death:
                mEyeFollowPlayer = false;

                bodyAnimator.Play(deathTake);
                break;
        }
    }

    protected override void SetBlink(bool blink) {
        foreach(SpriteColorPulse p in hurtPulses) {
            p.enabled = blink;
        }
    }

    protected override void OnDespawned() {
        //reset stuff here
        state = (int)State.Invalid;

        base.OnDespawned();
    }

    protected override void OnDestroy() {
        //dealloc here

        base.OnDestroy();
    }

    public override void SpawnFinish() {
        //start ai, player control, etc
        state = (int)State.Pattern0;
    }

    protected override void SpawnStart() {
        //initialize some things
        mStats.Reset();

        mEyeFollowPlayer = true;
    }

    protected override void Awake() {
        base.Awake();

        //initialize variables
        mStats = GetComponent<Stats>();

        foreach(SpriteColorPulse p in hurtPulses) {
            p.enabled = false;
        }

        mOrigPos = transform.position;

        tongue.SetActive(false);

        mPattern1FixedY = pattern1FixedY.position.y;

        pattern2TongueAnimator.gameObject.SetActive(false);

        mPattern2FixedY = pattern2FixedY.position.y;

        pattern3EyesRotate.SetActive(false);
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)
        mPlayer = Player.instance;
    }

    void FixedUpdate() {
        if(mEyeFollowPlayer) {
            Vector3 dir = mPlayer.transform.position - transform.position;
            dir.z = 0.0f;
            dir.Normalize();
            foreach(Transform eye in eyes) {
                eye.up = dir;
            }
        }

        Vector3 dest;
        Vector3 curPos = transform.position;
        float dt = Time.fixedDeltaTime;

        switch((State)state) {
            case State.Revert:
                mCurTime += dt;

                if(curPos == mOrigPos || mCurTime >= revertDelay) {
                    state = (int)State.Pattern0;
                }
                else {
                    transform.position = Vector3.SmoothDamp(curPos, mOrigPos, ref mCurFollowVel, revertDelay, dampMaxSpeed, dt);
                }
                break;

            case State.Pattern0:
                mCurTime += dt;

                if(mCurTime >= wait) {
                    //state = (int)State.Pattern3;
                    state = Random.Range((int)State.Pattern1, (int)State.MaxPatternRandom);
                }
                else {
                    transform.position = Vector3.SmoothDamp(curPos, mOrigPos, ref mCurFollowVel, revertDelay, dampMaxSpeed, dt);
                }
                break;

            case State.Pattern1:
                mCurTime += dt;

                if(mCurTime >= pattern1Delay) {
                    state = (int)State.Revert;
                }
                else {
                    Vector3 playerPos = mPlayer.transform.position;

                    dest = new Vector3(playerPos.x, mPattern1FixedY, mOrigPos.z);
                    transform.position = Vector3.SmoothDamp(curPos, dest, ref mCurFollowVel, pattern1FollowDelay, dampMaxSpeed, dt);
                }
                break;

            case State.Pattern2:
                dest = new Vector3(curPos.x, mPattern2FixedY, mOrigPos.z);
                transform.position = Vector3.SmoothDamp(curPos, dest, ref mCurFollowVel, pattern2FollowDelay, dampMaxSpeed, dt);

                if(mPattern2CurNumPlay < pattern2TongueNumPlay) {
                    float tDelay = mPattern2CurNumPlay > 0 ? pattern2TonguePlayDelay : pattern2TonguePlayDelayStart;
                    if(mCurTime < tDelay) {
                        Vector3 tonguePos = pattern2TongueAnimator.transform.position;
                        Vector3 tongueDest = new Vector3(mPlayer.transform.position.x, tonguePos.y, tonguePos.z);
                        pattern2TongueAnimator.transform.position = Vector3.SmoothDamp(tonguePos, tongueDest, ref mPattern2TongueCurVel, pattern2TongueFollowDelay, dampMaxSpeed, dt);

                        mCurTime += dt;

                        if(mCurTime >= pattern2TonguePlayDelay)
                            pattern2TongueAnimator.Play("play");
                    }
                    else if(!pattern2TongueAnimator.isPlaying) {
                        mPattern2TongueCurVel = Vector3.zero;
                        mCurTime = 0.0f;
                        mPattern2CurNumPlay++;
                    }
                }
                else {
                    pattern2TongueAnimator.gameObject.SetActive(false);
                    state = (int)State.Revert;
                }
                break;

            case State.Pattern3:
                DoSteer(pattern3Accel, pattern3SpeedCap);

                mCurTime += dt;

                if(mCurTime >= pattern3Delay) {
                    bodyAnimator.Play(pattern3EyesRevertTake);
                    state = (int)State.Revert;
                }
                break;

            case State.DeathPrepare:
                if(mCurTime <= deathRevertDelay) {
                    mCurTime += Time.deltaTime;
                    float t = Holoville.HOTween.Core.Easing.Quad.EaseOut(mCurTime, 0.0f, 1.0f, deathRevertDelay, 0, 0);
                    transform.position = Vector3.Lerp(mDeathPrepStartPos, mOrigPos, t);
                }
                else {
                    state = (int)State.Death;
                }
                break;

            case State.Death:
                if(!bodyAnimator.isPlaying) {
                    state = (int)State.Invalid;
                    Main.instance.sceneManager.LoadScene("victory");
                }
                break;
        }
    }

    void OnHit(HitTrigger hit) {
        switch((State)state) {
            case State.Invalid:
            case State.DeathPrepare:
            case State.Death:
                break;

            default:
                if(!mInvul && !isBlinking) {
                    mStats.hp -= hit.damage;

                    if(mStats.hp <= 0.0f) {
                        state = (int)State.DeathPrepare;
                    }
                    else {
                        Blink(hurtDelay);
                    }
                }
                break;
        }
    }

    void OnProjRelease(EntityBase ent) {
        mCurNumFireballs = 0;
        ent.releaseCallback -= OnProjRelease;
    }

    void ShootFireballFromMouth() {
        if(mCurNumFireballs < maxFireballs) {
            Vector3 dir = mPlayer.transform.position - transform.position;
            dir.z = 0.0f;
            dir.Normalize();

            Projectile proj = Projectile.Create(projGrp, projType, mouthPoint.position, dir, null);
            proj.releaseCallback += OnProjRelease;
        }
    }

    void DoSteer(float _accel, float _spdCap) {
        Vector3 dir = mPlayer.transform.position - transform.position;
        dir.z = 0.0f;
        dir.Normalize();

        //Debug.Log("spd: " + rigidbody.velocity.magnitude);
        rigidbody.AddForce(dir * _accel, ForceMode.Acceleration);

        float sqrMag = rigidbody.velocity.sqrMagnitude;
        if(sqrMag > _spdCap * _spdCap)
            rigidbody.velocity = (rigidbody.velocity / Mathf.Sqrt(sqrMag)) * _spdCap;
        //rigidbody.AddForce(mSteerToDir * _accel, ForceMode.Acceleration);
    }
}

using UnityEngine;
using System.Collections;

public class EnemySeekerController : MonoBehaviour {
    public float idleAccel = 10.0f;
    public float idleSpeedCap = 5.0f;
    public float idleDirDelay = 0.5f;

    public float accel = 10.0f;
    public float speedCap = 5.0f;

    public float seekRadius = 10.0f;
    public float seekDelay = 0.5f;

    public float orientSpeed = 90.0f;

    private Enemy mEnemy;
    private Player mPlayer;
    private Vector3 mStartPos;
    private Quaternion mStartRot;
    private WaitForFixedUpdate mWait;
    private float mSeekLastTime;

    private Vector3 mSteerToDir;

    void ApplyEnemyState() {
        StopAllCoroutines();

        switch((EntityState)mEnemy.state) {
            case EntityState.Normal:
                mEnemy.facePlayer = false;
                StartCoroutine(DoWander());
                break;

            case EntityState.Attack:
                mEnemy.facePlayer = true;
                StartCoroutine(DoSeek());
                break;

            case EntityState.Dead:
                rigidbody.velocity = Vector3.zero;
                break;
        }
    }

    void OnEnemyChangeState(EntityBase ent) {
        ApplyEnemyState();
    }

    void OnEnemyActivatorWake() {
        ApplyEnemyState();
    }

    // Use this for initialization
    void Start() {
        mEnemy = GetComponent<Enemy>();

        mEnemy.setStateCallback += OnEnemyChangeState;

        if(mEnemy.activator)
            mEnemy.activator.awakeCallback += OnEnemyActivatorWake;

        mPlayer = Player.instance;

        mStartPos = transform.position;
        mStartRot = transform.rotation;

        mWait = new WaitForFixedUpdate();
    }

    IEnumerator DoWander() {
        mSeekLastTime = 0.0f;

        while(true) {
            DoOrientation(mStartRot, Time.fixedDeltaTime);

            //movement
            DoSeekDir(mStartPos, Time.fixedTime, idleDirDelay);
            DoSteer(idleAccel, idleSpeedCap);

            //check if near player
            //make sure player is in normal or hurt
            if(mPlayer.state == (int)EntityState.Normal || mPlayer.state == (int)EntityState.Hurt || mPlayer.state == (int)EntityState.Attack) {
                Vector2 dpos = (mPlayer.transform.position - transform.position);
                float distSqr = dpos.SqrMagnitude();
                if(distSqr <= seekRadius * seekRadius) {
                    mEnemy.state = (int)EntityState.Attack;
                    break;
                }
            }

            yield return mWait;
        }
    }

    IEnumerator DoSeek() {
        mSeekLastTime = 0.0f;

        while(true) {
            Transform playerBodyTrans = mPlayer.transform;

            DoOrientation(playerBodyTrans.rotation, Time.fixedDeltaTime);

            //movement, if player is in normal
            if(mPlayer.state == (int)EntityState.Normal || mPlayer.state == (int)EntityState.Attack) {
                DoSeekDir(playerBodyTrans.position, Time.fixedTime, seekDelay);
                //Debug.Log("dir: " + mSteerToDir);
            }

            DoSteer(accel, speedCap);

            //check if outside player
            Vector2 dpos = (mPlayer.transform.position - transform.position);
            float distSqr = dpos.SqrMagnitude();
            if(distSqr > seekRadius * seekRadius) {
                mEnemy.state = (int)EntityState.Normal;
                break;
            }

            yield return mWait;
        }
    }

    void DoSteer(float _accel, float _spdCap) {
        //Debug.Log("spd: " + rigidbody.velocity.magnitude);
        rigidbody.AddForce(mSteerToDir * _accel, ForceMode.Acceleration);

        float sqrMag = rigidbody.velocity.sqrMagnitude;
        if(sqrMag > _spdCap * _spdCap)
            rigidbody.velocity = (rigidbody.velocity / Mathf.Sqrt(sqrMag)) * _spdCap;
            //rigidbody.AddForce(mSteerToDir * _accel, ForceMode.Acceleration);
    }

    void DoSeekDir(Vector3 toPos, float curTime, float _delay) {
        if(curTime - mSeekLastTime >= _delay) {
            mSteerToDir = toPos - transform.position;

            if(mSteerToDir == Vector3.zero) {
                mSteerToDir = Random.insideUnitCircle;
            }

            mSteerToDir.Normalize();

            mSeekLastTime = curTime;
        }
    }

    void DoOrientation(Quaternion toRot, float timeDelta) {
        Quaternion curRot = transform.rotation;
        if(curRot != toRot) {
            transform.rotation = Quaternion.RotateTowards(curRot, toRot, orientSpeed * timeDelta);
        }
    }

    void OnDrawGizmos() {
        if(seekRadius > 0) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, seekRadius);
        }
    }
}

using UnityEngine;
using System.Collections;

public class EnemyWanderController : RigidBodyController {
    public float waitDelay;
    public float minDistance;
    public float maxDistance;

    private Enemy mEnemy;
    private bool mActive = false;

    protected override void Awake() {
        base.Awake();

        mEnemy = GetComponent<Enemy>();
        mEnemy.setStateCallback += OnEntitySetState;

        if(mEnemy.activator)
            mEnemy.activator.awakeCallback += OnActivateWake;
    }

    void OnEntitySetState(EntityBase ent) {
        switch((EntityState)ent.state) {
            case EntityState.Normal:
                StartCoroutine(DoMove());
                break;

            case EntityState.Hurt:
            case EntityState.Dead:
            case EntityState.Invalid:
                moveSide = 0.0f;
                mActive = false;
                break;
        }
    }

    void OnActivateWake() {
        switch((EntityState)mEnemy.state) {
            case EntityState.Normal:
                StartCoroutine(DoMove());
                break;
        }
    }

    IEnumerator DoMove() {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        mActive = true;

        float waitTime = 0.0f;
        float moveDist = 0.0f;
        float lastX = 0.0f;

        moveSide = 0.0f;

        while(mActive) {
            if(waitTime < waitDelay) {
                waitTime += Time.fixedDeltaTime;

                if(waitTime > waitDelay) {
                    lastX = transform.position.x;
                    moveDist = Random.Range(minDistance, maxDistance);
                    moveSide = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                }
            }
            else {
                if(Mathf.Abs(transform.position.x - lastX) >= moveDist) {
                    moveSide = 0.0f;
                    waitTime = 0.0f;
                }
            }

            yield return wait;
        }
    }
}

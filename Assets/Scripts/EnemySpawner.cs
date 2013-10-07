using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public string group;
    public string type;

    public int maxSpawns;
    public float waitDelay;
    public float minDistance;
    public float maxDistance;

    public bool alwaysActive = false;

    private int mCurSpawns;
    private float mCurTime;
    private bool mWait;
    private bool mActive;

    void OnEnable() {
        mWait = false;

        if(alwaysActive)
            mActive = true;
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player") {
            mWait = false;
            mActive = true;
        }
    }

    void OnTriggerExit(Collider col) {
        if(col.tag == "Player" && !alwaysActive) {
            mActive = false;
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(mActive) {
            if(mWait) {
                mCurTime += Time.deltaTime;
                if(mCurTime >= waitDelay) {
                    if(mCurSpawns < maxSpawns)
                        mWait = false;
                    else
                        mCurTime = 0;
                }
            }
            else {
                Vector3 pos = transform.position;
                float ofsX = Random.Range(minDistance, maxDistance);
                pos.x += ofsX;
                Transform t = PoolController.Spawn(group, type, type, null, pos, Quaternion.identity);
                EntityBase ent = t.GetComponent<EntityBase>();
                ent.releaseCallback += OnEntityRelease;
                mCurSpawns++;

                mWait = true;
                mCurTime = 0;
            }
        }
    }

    void OnEntityRelease(EntityBase ent) {
        mCurSpawns--;
        ent.releaseCallback -= OnEntityRelease;
    }
}

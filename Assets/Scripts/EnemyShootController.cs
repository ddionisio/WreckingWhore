using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShootController : MonoBehaviour {
    public delegate void OnShoot(EnemyShootController ctrl);

    public Transform spawnPoint;

    public string group;
    public string type;
    public bool seekTarget;
    public int maxCount = 1;

    public Vector3 visionDir = new Vector3(1, 0, 0);
    public float visionAngleLim = 360;
    public LayerMask visionObscureMask;

    public event OnShoot shootCallback;

    private bool mShootEnable;
    private Ray mVisionRay;
    private HashSet<Projectile> mLaunchedProjs = new HashSet<Projectile>();

    public bool shootEnable {
        get { return mShootEnable; }
        set { mShootEnable = value; }
    }

    public void Shoot(Vector3 dir, Transform target) {
        Shoot(spawnPoint ? spawnPoint.position : transform.position, dir, target);
    }

    public void Shoot(Vector3 pos, Vector3 dir, Transform target) {
        if(string.IsNullOrEmpty(type))
            return;

        Projectile proj = Projectile.Create(group, type, pos, dir, target);

        if(proj) {
            mLaunchedProjs.Add(proj);
            proj.releaseCallback += OnProjRelease;

            if(shootCallback != null)
                shootCallback(this);
        }
    }

    public void ClearProjectiles() {
        //release any projectiles that are currently spawning
        foreach(Projectile proj in mLaunchedProjs) {
            if(proj && proj.spawning) {
                proj.Release();
            }
        }

        //no longer need to track projectiles
        mLaunchedProjs.Clear();
    }

    void OnTriggerStay(Collider col) {
        if(mShootEnable && mLaunchedProjs.Count < maxCount) {
            Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
            Transform seek = col.transform;

            Vector3 dir = seek.position - pos;
            float dist = dir.magnitude;
            dir /= dist;

            //check if within angle
            if(visionAngleLim < 360) {
                Vector3 checkDir = transform.rotation * visionDir;
                if(Vector3.Angle(checkDir, dir) > visionAngleLim)
                    return;
            }

            //check walls
            mVisionRay.origin = pos;
            mVisionRay.direction = dir;
            if(Physics.Raycast(mVisionRay, dist, visionObscureMask))
                return;

            Shoot(pos, dir, seekTarget ? seek : null);
        }
    }

    void OnDestroy() {
        shootCallback = null;
    }

    void OnProjRelease(EntityBase ent) {
        ent.releaseCallback -= OnProjRelease;

        mLaunchedProjs.Remove(ent as Projectile);
    }
}

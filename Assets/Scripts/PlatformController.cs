using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour {
    public string[] tags;
    public LayerMask layerMask;

    //public float velocityAngleDiff = 89.0f;
    //public float normalAngleDiff = 90.0f;

    public enum Dir {
        Up,
        Down,
        Left,
        Right
    }

    public Dir dir = Dir.Up;
    public float ofs = 0.15f;

    public bool upDirLimitEnabled = false; //check collider's up dir angle from this platform's dir
    public float upDirLimit = 15.0f;

    Vector3 mDir;

    HashSet<PlatformerController> mPlatformers = new HashSet<PlatformerController>();
    HashSet<PlatformerController> mCurPlatformerSweep = new HashSet<PlatformerController>();

    bool CheckTags(GameObject go) {
        foreach(string tag in tags) {
            if(go.tag == tag)
                return true;
        }

        return false;
    }

    void SetDir() {
        switch(dir) {
            case Dir.Up:
                mDir = Vector3.up;
                break;
            case Dir.Down:
                mDir = -Vector3.up;
                break;
            case Dir.Left:
                mDir = -Vector3.right;
                break;
            case Dir.Right:
                mDir = Vector3.right;
                break;
        }
    }

    void OnDisable() {
        mPlatformers.Clear();
        mCurPlatformerSweep.Clear();
    }

    void Awake() {
        SetDir();
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
#if UNITY_EDITOR
        SetDir();
#endif
        Vector3 vel = rigidbody.velocity;// GetPointVelocity(hit.point);

        if(vel != Vector3.zero) {
            Vector3 wDir = transform.rotation * mDir;

            RaycastHit[] hits = rigidbody.SweepTestAll(wDir, ofs);

            foreach(RaycastHit hit in hits) {
                GameObject go = hit.collider.gameObject;
                Rigidbody body = go.rigidbody;
                //Vector3 up = go.transform.up;

                if(((1 << go.layer) & layerMask) != 0 && CheckTags(go) && (!upDirLimitEnabled || Vector3.Angle(wDir, hit.transform.up) <= upDirLimit)) {// && Vector3.Angle(up, hit.normal) >= normalAngleDiff) {

                    PlatformerController ctrl = go.GetComponent<PlatformerController>();

                    bool jumping = ctrl != null && (ctrl.isJump || ctrl.isJumpWall);

                    Vector3 localV = go.transform.worldToLocalMatrix.MultiplyVector(vel);
                    Vector3 nLocalV = jumping ? new Vector3(0, localV.y > 0 ? localV.y : 0) : new Vector3(localV.x, localV.y < 0 ? localV.y : 0);
                    Vector3 nWorldV = go.transform.localToWorldMatrix.MultiplyVector(nLocalV);

                    if(jumping) {
                        if(!mPlatformers.Contains(ctrl))
                            body.velocity += nWorldV;
                    }
                    else {
                        body.MovePosition(go.transform.position + nWorldV * Time.fixedDeltaTime);
                    }

                    //body.velocity += go.transform.localToWorldMatrix.MultiplyVector(nLocalV);

                    /*if(velocityAngleDiff == 0 || body.velocity == Vector3.zero || Vector3.Angle(wDir, vel) >= velocityAngleDiff) {
                        body.MovePosition(go.transform.position + vel * Time.fixedDeltaTime);
                        //body.velocity += vel;
                    }*/

                    if(ctrl && !jumping) {
                        mCurPlatformerSweep.Add(ctrl);
                        ctrl._PlatformSweep(true, gameObject.layer);
                    }
                }
            }
        }

        foreach(PlatformerController ctrl in mPlatformers) {
            if(!mCurPlatformerSweep.Contains(ctrl))
                ctrl._PlatformSweep(false, gameObject.layer);
        }

        HashSet<PlatformerController> prev = mPlatformers;
        mPlatformers = mCurPlatformerSweep;
        mCurPlatformerSweep = prev;
        mCurPlatformerSweep.Clear();
    }
}

using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {
    public Transform target;
    public float delay;
    public Vector3 ofs;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    private Vector3 mCurVel;

    void OnEnable() {
        if(target) {
            transform.position = target.position + ofs;
        }

        mCurVel = Vector3.zero;
    }

    // Use this for initialization
    void Start() {
        if(target) {
            transform.position = target.position + ofs;
        }

        mCurVel = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        if(target) {
            Vector3 curPos = transform.position;
            Vector3 toPos = target.position;
            if(lockX) toPos.x = curPos.x;
            if(lockY) toPos.y = curPos.y;
            if(lockZ) toPos.z = curPos.z;
            transform.position = Vector3.SmoothDamp(curPos, toPos, ref mCurVel, delay, Mathf.Infinity, Time.deltaTime);
        }
    }
}

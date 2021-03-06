using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/AnimSpinner")]
public class TransAnimSpinner : MonoBehaviour {
    public Vector3 rotatePerSecond;
    public bool local = true;
    public bool forceFixedUpdate;

    private Vector3 mEulerAnglesOrig;

    void OnEnable() {
        mEulerAnglesOrig = transform.eulerAngles;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(GetComponent<Rigidbody>() == null) {
            if(local) {
                transform.localEulerAngles = transform.localEulerAngles + rotatePerSecond * Time.deltaTime;
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * Time.deltaTime;
                transform.eulerAngles = mEulerAnglesOrig;
            }
        }
    }

    void FixedUpdate() {
        if(GetComponent<Rigidbody>() != null) {
            if(local) {
                Vector3 eulers = transform.eulerAngles;
                GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(eulers + rotatePerSecond * Time.fixedDeltaTime));
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * Time.fixedDeltaTime;
                GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(mEulerAnglesOrig));
            }
        }
        else if(forceFixedUpdate) {
            if(local) {
                transform.localEulerAngles = transform.localEulerAngles + rotatePerSecond * Time.fixedDeltaTime;
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * Time.fixedDeltaTime;
                transform.eulerAngles = mEulerAnglesOrig;
            }
        }
    }
}

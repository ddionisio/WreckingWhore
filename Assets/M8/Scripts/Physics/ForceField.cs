﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Physics/ForceField")]
public class ForceField : MonoBehaviour {
    public enum Axis {
        Up,
        Forward,
        Right
    }

    public enum Mode {
        Dir,
        Center,
    }

    [SerializeField]
    Mode _mode;

    [SerializeField]
    Axis _dir;

    public bool inverse;

    public float force;
    public float impulse;

    [SerializeField]
    float _updateDelay = 0.2f;

    private bool mModeRunning = false;
    private YieldInstruction mWait;
    private Vector3 mCenterLocal;
    private HashSet<Rigidbody> mBodies = new HashSet<Rigidbody>();

    void OnTriggerEnter(Collider t) {
        Rigidbody body = t.GetComponent<Rigidbody>();
        if(body != null && !mBodies.Contains(body)) {
            if(impulse != 0.0f) {
                Vector3 dir = Vector3.zero;
                switch(_mode) {
                    case Mode.Dir:
                        switch(_dir) {
                            case Axis.Right:
                                dir = Vector3.right;
                                break;
                            case Axis.Forward:
                                dir = Vector3.forward;
                                break;
                            default:
                                dir = Vector3.up;
                                break;
                        }
                        break;

                    case Mode.Center:
                        Vector3 pos = transform.localToWorldMatrix.MultiplyPoint(mCenterLocal);
                        dir = inverse ? pos - body.position : body.position - pos;
                        dir.Normalize();
                        break;
                }

                body.AddForce(dir * impulse, ForceMode.Impulse);
            }

            mBodies.Add(body);

            StartRoutine();
        }
    }

    void OnTriggerExit(Collider t) {
        Rigidbody body = t.GetComponent<Rigidbody>();
        if(body != null) {
            mBodies.Remove(body);
        }
    }

    void OnDisable() {
        mBodies.Clear();
        mModeRunning = false;
        StopAllCoroutines();
    }

    void Awake() {
        if(_updateDelay > 0.0f)
            mWait = new WaitForSeconds(_updateDelay);
        else
            mWait = new WaitForFixedUpdate();

        //compute center
        mCenterLocal = Vector3.zero;

        Collider[] cols = GetComponentsInChildren<Collider>();
        if(cols != null && cols.Length > 0) {
            foreach(Collider col in cols) {
                mCenterLocal += col.bounds.center;
            }

            mCenterLocal /= cols.Length;

            mCenterLocal = transform.worldToLocalMatrix.MultiplyPoint(mCenterLocal);
        }
    }

    void StartRoutine() {
        if(!mModeRunning) {
            switch(_mode) {
                case Mode.Center:
                    StartCoroutine(DoModeCenter());
                    break;

                case Mode.Dir:
                    StartCoroutine(DoModeDir());
                    break;
            }
        }
    }

    IEnumerator DoModeDir() {
        mModeRunning = true;

        while(mBodies.Count > 0) {
            Vector3 dir;
            switch(_dir) {
                case Axis.Right:
                    dir = Vector3.right;
                    break;
                case Axis.Forward:
                    dir = Vector3.forward;
                    break;
                default:
                    dir = Vector3.up;
                    break;
            }

            dir = transform.rotation * (inverse ? -dir : dir);

            foreach(Rigidbody body in mBodies) {
                body.AddForce(dir * force, ForceMode.Force);
            }

            yield return mWait;
        }

        mModeRunning = false;
    }

    IEnumerator DoModeCenter() {
        mModeRunning = true;

        while(mBodies.Count > 0) {
            Vector3 pos = transform.localToWorldMatrix.MultiplyPoint(mCenterLocal);

            foreach(Rigidbody body in mBodies) {
                Vector3 dir = inverse ? pos - body.position : body.position - pos;
                dir.Normalize();

                body.AddForce(dir * force, ForceMode.Force);
            }

            yield return mWait;
        }

        mModeRunning = false;
    }
}

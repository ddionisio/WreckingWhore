﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
[AddComponentMenu("M8/Physics/First-Person RigidBodyController")]
public class FPController : RigidBodyController {
    [SerializeField]
    Transform _eye;

    public Vector3 eyeOfs;
    public float eyeLockOrientSpeed = 1080.0f; //when we lock the eye again, this is the speed to re-orient based on dirHolder
    public float eyeLockPositionDelay = 0.01f; //reposition delay when we lock the eye again

    public bool moveNormalized = true; //use false for analogue

    public float jumpImpulse = 4.0f;
    public float jumpWaterForce = 40.0f;
    public float jumpForce = 15.0f;
    public float jumpDelay = 0.15f;

    public bool jumpWall = false; //wall jump

    public float lookSensitivity = 4.0f;
    public bool lookYInvert = false;
    public float lookYAngleMin = -90.0f;
    public float lookYAngleMax = 90.0f;

    public string ladderTag = "Ladder";
    public LayerMask ladderLayer;
    public float ladderOrientSpeed = 270.0f;
    public float ladderDrag = 15.0f;
    public float ladderJumpForce = 30.0f;

    public int player = 0;
    public int moveInputX = InputManager.ActionInvalid;
    public int moveInputY = InputManager.ActionInvalid;
    public int lookInputX = InputManager.ActionInvalid;
    public int lookInputY = InputManager.ActionInvalid;
    public int jumpInput = InputManager.ActionInvalid;

    public bool startInputEnabled = false;

    private bool mInputEnabled = false;

    private bool mJump = false;
    private float mJumpLastTime = 0.0f;

    private Vector2 mLookCurInputAxis;
    private float mLookCurRot;

    private int mLadderCounter;
    private bool mLadderLastGravity;
    private Vector3 mLadderUp;
    private Quaternion mLadderRot;

    private bool mEyeLocked = true;
    private bool mEyeOrienting = false;
    private Vector3 mEyeOrientVel;
    
    public bool inputEnabled {
        get { return mInputEnabled; }
        set {
            if(mInputEnabled != value) {
                mInputEnabled = value;

                InputManager input = Main.instance != null ? Main.instance.input : null;
                if(input != null) {
                    if(mInputEnabled) {
                        input.AddButtonCall(player, jumpInput, OnInputJump);
                    }
                    else {
                        input.RemoveButtonCall(player, jumpInput, OnInputJump);
                    }
                }
            }
        }
    }

    public bool isOnLadder { get { return mLadderCounter > 0; } }

    public Transform eye {
        get { return _eye; }
    }

    /// <summary>
    /// This determines whether or not the eye will be set to the dirHolder's transform.
    /// default: true. If false, input for looking up/down will be disabled.
    /// </summary>
    public bool eyeLocked {
        get { return _eye != null && mEyeLocked; }
        set {
            if(mEyeLocked != value && _eye != null) {
                mEyeLocked = value;

                if(mEyeLocked) {
                    //move eye orientation to dirHolder
                    EyeOrient();
                }
                else {
                    mLookCurRot = 0.0f;
                    mEyeOrienting = false;
                }
            }
        }
    }

    public override void ResetCollision() {
        base.ResetCollision();

        if(mLadderCounter > 0) {
            if(gravityController != null)
                gravityController.enabled = true;
            else
                rigidbody.useGravity = mLadderLastGravity;

            mLadderCounter = 0;
        }
    }

    protected override void WaterExit() {
        if(mJump) {
            if(jumpImpulse > 0.0f)
                rigidbody.AddForce(dirHolder.up * jumpImpulse, ForceMode.Impulse);

            mJumpLastTime = Time.fixedTime;
        }
    }

    protected override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            mLadderUp = col.transform.up;

            if(!M8.MathUtil.RotateToUp(mLadderUp, transform.right, transform.forward, ref mLadderRot))
                transform.up = mLadderUp;

            mLadderCounter++;
        }

        if(isOnLadder) {
            if(gravityController != null) {
                StartCoroutine(LadderOrientUp());
                gravityController.enabled = false;
            }
            else {
                mLadderLastGravity = rigidbody.useGravity;
                rigidbody.useGravity = false;
            }
        }
    }

    protected override void OnTriggerStay(Collider col) {
        base.OnTriggerStay(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            if(mLadderCounter == 0) {
                mLadderCounter++;

                if(gravityController != null) {
                    StartCoroutine(LadderOrientUp());
                    gravityController.enabled = false;
                }
                else {
                    mLadderLastGravity = rigidbody.useGravity;
                    rigidbody.useGravity = false;
                }
            }

            if(mLadderUp != col.transform.up) {
                mLadderUp = col.transform.up;

                if(!M8.MathUtil.RotateToUp(mLadderUp, transform.right, transform.forward, ref mLadderRot))
                    transform.up = mLadderUp;
            }
        }
    }

    protected override void OnTriggerExit(Collider col) {
        base.OnTriggerExit(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            mLadderCounter--;
        }

        if(!isOnLadder) {
            if(gravityController != null)
                gravityController.enabled = true;
            else
                rigidbody.useGravity = mLadderLastGravity;
        }
    }

    protected override void OnDestroy() {
        inputEnabled = false;

        base.OnDestroy();
    }

    protected override void OnDisable() {
        base.OnDisable();

        mEyeOrienting = false;
        mLookCurRot = 0.0f;
    }

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        inputEnabled = startInputEnabled;
    }

    // Update is called once per frame
    protected override void FixedUpdate() {
        Rigidbody body = rigidbody;
        Quaternion dirRot = dirHolder.rotation;

        if(mInputEnabled) {
            InputManager input = Main.instance.input;

            float moveX, moveY;

            if(moveNormalized) {
                Vector2 moveVec = new Vector2(
                    moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f,
                    moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f);

                moveVec.Normalize();

                moveX = moveVec.x;
                moveY = moveVec.y;

            }
            else {
                moveX = moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f;
                moveY = moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f;
            }

            //movement
            moveForward = 0.0f;
            moveSide = 0.0f;

            if(isOnLadder) {
                //move forward upwards
                Move(dirRot, Vector3.up, Vector3.right, new Vector2(moveX, moveY), moveForce);
            }
            else if(isUnderWater && !isGrounded) {
                //Move based on eye
                Move(_eye.rotation, Vector3.forward, Vector3.right, new Vector2(moveX, moveY), moveForce);
            }
            else if(!isSlopSlide) {
                moveForward = moveY;
                moveSide = moveX;
            }
                        
            //look
            if(mEyeLocked && !mEyeOrienting) {
                mLookCurInputAxis.x = lookInputX != InputManager.ActionInvalid ? input.GetAxis(player, lookInputX) : 0.0f;
                mLookCurInputAxis.y = lookInputY != InputManager.ActionInvalid ? input.GetAxis(player, lookInputY) : 0.0f;

                //horizontal
                if(mLookCurInputAxis.x != 0.0f) {
                    dirRot *= Quaternion.AngleAxis(mLookCurInputAxis.x * lookSensitivity, Vector3.up);
                    dirHolder.rotation = dirRot;
                }

                //vertical
                if(mLookCurInputAxis.y != 0.0f) {
                    float vDelta = mLookCurInputAxis.y * lookSensitivity;

                    mLookCurRot += vDelta;

                    if(mLookCurRot < -360.0f)
                        mLookCurRot += 360.0f;
                    else if(mLookCurRot > 360.0f)
                        mLookCurRot -= 360.0f;

                    mLookCurRot = Mathf.Clamp(mLookCurRot, lookYAngleMin, lookYAngleMax);
                }
            }

            //jump
            if(mJump) {
                if(isOnLadder) {
                    body.AddForce(dirRot * Vector3.up * ladderJumpForce);
                }
                else if(isUnderWater) {
                    body.AddForce(dirRot * Vector3.up * jumpWaterForce);
                }
                else {
                    if(Time.fixedTime - mJumpLastTime >= jumpDelay || (collisionFlags & CollisionFlags.Above) != 0) {
                        mJump = false;
                    }
                    else {
                        body.AddForce(dirRot * Vector3.up * jumpForce);
                    }
                }
            }
        }
        else {
            moveForward = 0.0f;
            moveSide = 0.0f;
            mLookCurInputAxis = Vector2.zero;
            mJump = false;
        }

        //set eye rotation
        if(_eye != null && mEyeLocked && !mEyeOrienting) {
            Quaternion rot = dirHolder.rotation;

            if(mLookCurRot != 0.0f)
                rot *= Quaternion.AngleAxis(lookYInvert ? mLookCurRot : -mLookCurRot, Vector3.right);

            _eye.rotation = rot;

            Vector3 pos = dirHolder.position;

            if(eyeOfs.x != 0.0f)
                pos += dirHolder.right * eyeOfs.x;

            if(eyeOfs.y != 0.0f)
                pos += dirHolder.up * eyeOfs.y;

            if(eyeOfs.z != 0.0f)
                pos += dirHolder.forward * eyeOfs.z;

            _eye.position = pos;
        }

        base.FixedUpdate();

        if(isOnLadder)
            rigidbody.drag = ladderDrag;
    }

    void OnInputJump(InputManager.Info dat) {
        //jumpWall
        if(dat.state == InputManager.State.Pressed) {
            if(isUnderWater || isOnLadder) {
                mJump = true;
            }
            else if(jumpWall && collisionFlags == CollisionFlags.Sides) {
                bool found = false;
                CollideInfo inf = new CollideInfo();

                for(int i = 0; i < mCollCount; i++) {
                    CollideInfo cInf = mColls[i];
                    if(cInf.flag == CollisionFlags.Sides) {
                        if(Vector3.Angle(dirHolder.forward, cInf.normal) > 120.0f) {
                            inf = cInf;
                            found = true;
                            break;
                        }
                    }
                }

                if(found) {
                    Vector3 jumpDir = inf.normal + dirHolder.up;

                    if(jumpImpulse > 0.0f) {
                        ClearCollFlags();
                        rigidbody.drag = airDrag;
                        rigidbody.AddForce(jumpDir * jumpImpulse, ForceMode.Impulse);
                    }

                    mJump = true;
                    mJumpLastTime = Time.fixedTime;

                    float a = M8.MathUtil.AngleForwardAxisDir(dirHolder.worldToLocalMatrix, Vector3.forward, inf.normal);
                    dirHolder.rotation *= Quaternion.AngleAxis(a, Vector3.up);
                    EyeOrient();
                    //TurnTo(inf.normal);
                }
            }
            else if(!mJump) {
                if(isUnderWater || isOnLadder) {
                    mJump = true;
                }
                else if(!isSlopSlide) {
                    if(isGrounded) {
                        if(jumpImpulse > 0.0f) {
                            ClearCollFlags();
                            rigidbody.drag = airDrag;
                            rigidbody.AddForce(dirHolder.up * jumpImpulse, ForceMode.Impulse);
                        }

                        mJump = true;
                        mJumpLastTime = Time.fixedTime;
                    }
                }
            }
        }
        else if(dat.state == InputManager.State.Released) {
            mJump = false;
        }
    }

    void EyeOrient() {
        //move eye orientation to dirHolder
        if(!mEyeOrienting) {
            mEyeOrienting = true;
            StartCoroutine(EyeOrienting());
        }

        mEyeOrientVel = Vector3.zero;
        mLookCurRot = 0;
    }

    IEnumerator LadderOrientUp() {
        WaitForFixedUpdate waitUpdate = new WaitForFixedUpdate();

        while(isOnLadder) {
            if(transform.up != mLadderUp) {
                float step = ladderOrientSpeed * Time.fixedDeltaTime;
                rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mLadderRot, step));
            }

            yield return waitUpdate;
        }
    }

    IEnumerator EyeOrienting() {
        WaitForFixedUpdate waitUpdate = new WaitForFixedUpdate();

        while(mEyeOrienting) {
            yield return waitUpdate;

            bool posDone = _eye.position == dirHolder.position;
            if(!posDone) {
                _eye.position = Vector3.SmoothDamp(_eye.position, dirHolder.position, ref mEyeOrientVel, eyeLockPositionDelay, Mathf.Infinity, Time.fixedDeltaTime);
            }

            bool rotDone = _eye.rotation == dirHolder.rotation;
            if(!rotDone) {
                float step = eyeLockOrientSpeed * Time.fixedDeltaTime;
                _eye.rotation = Quaternion.RotateTowards(_eye.rotation, dirHolder.rotation, step);
            }

            mEyeOrienting = !rotDone;
        }
    }
}

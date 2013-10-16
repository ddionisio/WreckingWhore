using UnityEngine;
using System.Collections;

public class WeaponBeam : Weapon {
    public tk2dBaseSprite cannon;
    public Transform beamHolder;
    public ParticleSystem particleEdge;
    public BoxCollider harmBodyBoxColl;

    public LayerMask collisionMask;

    public float maxDistance = 100.0f;
    public float forceBack = 20.0f;
    public float speedCap = 50.0f;

    private PlatformerController mOwnerCtrl;
    private Vector3 mBeamDir;
    private Transform[] mBeams;
    private Vector3 mLastParticlePos;

    public override void SetDir(Dir dir) {
        base.SetDir(dir);

        switch(mDir) {
            case Dir.Up:
                cannon.FlipX = false;
                cannon.transform.up = Vector3.left;
                mBeamDir = Vector3.up;
                particleEdge.transform.up = Vector3.left;
                particleEdge.transform.localScale = Vector3.one;
                break;
            case Dir.Down:
                cannon.FlipX = false;
                cannon.transform.up = Vector3.right;
                mBeamDir = Vector3.down;
                particleEdge.transform.up = Vector3.right;
                particleEdge.transform.localScale = Vector3.one;
                break;
            case Dir.Left:
                cannon.FlipX = true;
                cannon.transform.up = Vector3.up;
                mBeamDir = Vector3.left;
                particleEdge.transform.up = Vector3.up;
                particleEdge.transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Dir.Right:
                cannon.FlipX = false;
                cannon.transform.up = Vector3.up;
                mBeamDir = Vector3.right;
                particleEdge.transform.up = Vector3.up;
                particleEdge.transform.localScale = Vector3.one;
                break;
        }
    }

    public override void Cancel() {
        base.Cancel();

        _Active(mActive);

        //mOwnerCtrl.lockDrag = false;
    }

    public override void Attack() {
        base.Attack();

        _Active(mActive);

        //mOwnerCtrl.lockDrag = true;
        //mOwnerCtrl.rigidbody.drag = 0.0f;
    }

    void _Active(bool _active) {
        for(int i = 0, max = mBeams.Length; i < max; i++) {
            mBeams[i].gameObject.SetActive(_active);
        }

        harmBodyBoxColl.gameObject.SetActive(_active);
        particleEdge.loop = _active;

        if(_active) {
            FixedUpdate();
            particleEdge.Play();
        }
        else
            mLastParticlePos = particleEdge.transform.position;
    }

    void Awake() {
        mBeams = new Transform[beamHolder.childCount];
        for(int i = 0, max = mBeams.Length; i < max; i++) {
            mBeams[i] = beamHolder.GetChild(i);
        }

        _Active(false);

        mOwnerCtrl = owner.GetComponentInChildren<PlatformerController>();
    }

    // Use this for initialization
    void Start() {

    }

    void SetBeamWidth(float l) {
        for(int i = 0, max = mBeams.Length; i < max; i++) {
            if(mBeams[i] != harmBodyBoxColl.transform) { //ugh
                Vector3 s = mBeams[i].localScale;
                s.x = l;
                mBeams[i].localScale = s;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(mActive) {
            float len = 0.0f;
            RaycastHit hit;
            if(Physics.Raycast(beamHolder.position, mBeamDir, out hit, maxDistance, collisionMask)) {
                len = hit.distance;
            }
            else {
                len = maxDistance;
            }

            float hLen = len * 0.5f;

            Vector3 collSize = harmBodyBoxColl.size;
            collSize.x = len;
            harmBodyBoxColl.size = collSize;

            Vector3 collCenter = harmBodyBoxColl.transform.localPosition;
            collCenter.x = hLen;
            harmBodyBoxColl.transform.localPosition = collCenter;

            SetBeamWidth(len);

            if(!mOwnerCtrl.isGrounded || mDir == Dir.Down) {
                Vector3 vel = mOwnerCtrl.rigidbody.velocity;
                float spdSqr = vel.sqrMagnitude;
                if(spdSqr > speedCap * speedCap) {
                    mOwnerCtrl.rigidbody.velocity = (vel / Mathf.Sqrt(spdSqr)) * speedCap;
                }

                if(mOwnerCtrl.isGrounded)
                    mOwnerCtrl.rigidbody.drag = 0;

                Vector3 edgePos = particleEdge.transform.position;
                Vector3 edgeNPos = beamHolder.position + mBeamDir * len;
                edgeNPos.z = edgePos.z;
                particleEdge.transform.position = edgeNPos;

                mOwnerCtrl.rigidbody.AddForce(-mBeamDir * forceBack);
            }
        }
        else {
            if(particleEdge.isPlaying)
                particleEdge.transform.position = mLastParticlePos;
        }
    }
}

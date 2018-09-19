using UnityEngine;
using System.Collections;

public class Projectile : EntityBase {
    public enum State {
        Invalid = -1,
        Active,
        Seek,
        Dying
    }

    public enum ContactType {
        None,
        End,
        Stop,
        Bounce
    }

    public string[] hitTags;

    public float startVelocity;
    public float startVelocityAddRand;

    public float force;

    public float seekStartDelay = 0.0f;
    public float seekVelocity;
    public float seekVelocityCap = 5.0f;
    public float seekAngleCap = 360.0f;

    public float decayDelay;

    public bool releaseOnDie;
    public float dieDelay;

    public LayerMask explodeMask;
    public float explodeForce;
    public float explodeRadius;

    public float damage;

    public ContactType contactType = ContactType.End;

    public bool explodeOnDeath;

    public bool applyDirToUp;

    /*public bool oscillate;
    public float oscillateForce;
    public float oscillateDelay;*/

    private Vector3 mActiveForce;
    private Vector3 mStartDir = Vector3.zero;
    private Transform mSeek = null;
    private bool mSpawning = false;

    //private Vector2 mOscillateDir;
    //private bool mOscillateSwitch;

    public static Projectile Create(string group, string typeName, Vector3 startPos, Vector3 dir, Transform seek) {
        Projectile ret = Spawn<Projectile>(group, typeName, startPos);
        if(ret != null) {
            ret.mStartDir = dir;
            ret.seek = seek;
        }

        return ret;
    }

    public Transform seek {
        get { return mSeek; }
        set {
            mSeek = value;
        }
    }

    public bool spawning { get { return mSpawning; } }

    protected override void OnDespawned() {
        CancelInvoke();

        base.OnDespawned();
    }

    protected override void Awake() {
        base.Awake();

        if(GetComponent<Rigidbody>() != null) {
            GetComponent<Rigidbody>().detectCollisions = false;
        }

        if(GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    public override void SpawnFinish() {
        mSpawning = false;

        if(decayDelay == 0) {
            OnDecayEnd();
        }
        else {
            Invoke("OnDecayEnd", decayDelay);

            if(seekStartDelay > 0.0f) {
                state = (int)State.Active;

                Invoke("OnSeekStart", seekStartDelay);
            }
            else {
                OnSeekStart();
            }

            //starting direction and force
            if(GetComponent<Rigidbody>() != null && mStartDir != Vector3.zero) {
                //set velocity
                if(startVelocityAddRand != 0.0f) {
                    GetComponent<Rigidbody>().velocity = mStartDir * (startVelocity + Random.value * startVelocityAddRand);
                }
                else {
                    GetComponent<Rigidbody>().velocity = mStartDir * startVelocity;
                }

                mActiveForce = mStartDir * force;
            }

            if(applyDirToUp) {
                transform.up = mStartDir;
                InvokeRepeating("OnUpUpdate", 0.1f, 0.1f);
            }
        }
    }

    protected override void SpawnStart() {
        if(applyDirToUp && mStartDir != Vector3.zero) {
            transform.up = mStartDir;
        }

        mSpawning = true;
    }

    public override void Release() {
        state = (int)State.Invalid;

        mSpawning = false;

        base.Release();
    }

    protected override void StateChanged() {
        switch((State)state) {
            case State.Seek:
            case State.Active:
                if(GetComponent<Collider>())
                    GetComponent<Collider>().enabled = true;

                if(GetComponent<Rigidbody>())
                    GetComponent<Rigidbody>().detectCollisions = true;
                break;

            case State.Dying:
                CancelInvoke();

                if(GetComponent<Collider>())
                    GetComponent<Collider>().enabled = false;

                if(GetComponent<Rigidbody>()) {
                    GetComponent<Rigidbody>().detectCollisions = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                if(explodeOnDeath && explodeRadius > 0.0f) {
                    DoExplode();
                }

                if(releaseOnDie)
                    Invoke("Release", dieDelay);
                break;

            case State.Invalid:
                if(GetComponent<Collider>())
                    GetComponent<Collider>().enabled = false;

                if(GetComponent<Rigidbody>()) {
                    GetComponent<Rigidbody>().detectCollisions = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
        }
    }

    bool CheckTag(string tag) {
        if(hitTags.Length == 0)
            return true;

        for(int i = 0, max = hitTags.Length; i < max; i++) {
            if(hitTags[i] == tag)
                return true;
        }

        return false;
    }

    void ApplyContact(GameObject go, Vector3 normal) {
        switch(contactType) {
            case ContactType.End:
                state = (int)State.Dying;
                break;

            case ContactType.Stop:
                if(GetComponent<Rigidbody>() != null)
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                break;

            case ContactType.Bounce:
                if(GetComponent<Rigidbody>() != null) {
                    GetComponent<Rigidbody>().velocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, normal);
                }
                break;
        }

        //do damage
        if(!explodeOnDeath && CheckTag(go.tag)) {
            go.SendMessage("OnProjectileHit", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnCollisionEnter(Collision collision) {
        foreach(ContactPoint cp in collision.contacts) {
            ApplyContact(cp.otherCollider.gameObject, cp.normal);
        }

    }

    /*void OnTrigger(Collider collider) {
        ApplyContact(collider.gameObject, -mover.dir);
    }*/

    void OnDecayEnd() {
        state = (int)State.Dying;
    }

    void OnSeekStart() {
        state = (int)State.Seek;
    }

    void OnUpUpdate() {
        if(GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().velocity != Vector3.zero) {
            transform.up = GetComponent<Rigidbody>().velocity;
        }
    }

    void FixedUpdate() {
        switch((State)state) {
            case State.Active:
                if(GetComponent<Rigidbody>() != null)
                    GetComponent<Rigidbody>().AddForce(mActiveForce);
                break;

            case State.Seek:
                if(GetComponent<Rigidbody>() != null && mSeek != null) {
                    //steer torwards seek
                    Vector3 pos = transform.position;
                    Vector3 dest = mSeek.position;
                    Vector3 _dir = dest - pos;
                    float dist = _dir.magnitude;

                    if(dist > 0.0f) {
                        _dir /= dist;

                        //restrict
                        if(seekAngleCap < 360.0f) {
                            _dir = M8.MathUtil.DirCap(GetComponent<Rigidbody>().velocity.normalized, _dir, seekAngleCap);
                        }

                        Vector3 force = M8.MathUtil.Steer(GetComponent<Rigidbody>().velocity, _dir * seekVelocity, seekVelocityCap, 1.0f);
                        GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
                    }
                }
                break;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }

    private void DoExplode() {
        Vector3 pos = transform.position;
        //float explodeRadiusSqr = explodeRadius * explodeRadius;

        //TODO: spawn fx

        Collider[] cols = Physics.OverlapSphere(pos, explodeRadius, explodeMask.value);

        foreach(Collider col in cols) {
            if(col != null && col.GetComponent<Rigidbody>() != null && CheckTag(col.gameObject.tag)) {
                //hurt?
                col.GetComponent<Rigidbody>().AddExplosionForce(explodeForce, pos, explodeRadius, 0.0f, ForceMode.Force);

                //float distSqr = (col.transform.position - pos).sqrMagnitude;

                col.gameObject.SendMessage("OnProjectileHit", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

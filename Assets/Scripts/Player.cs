using UnityEngine;
using System.Collections;

public class Player : EntityBase {
    private static Player mInstance;

    private PlayerController mCtrl;
    private Stats mStats;

    public Stats stats { get { return mStats; } }
    public PlayerController controller { get { return mCtrl; } }

    protected override void OnDespawned() {
        //reset stuff here
        mStats.Reset();

        mCtrl.inputEnabled = false;

        base.OnDespawned();
    }

    protected override void OnDestroy() {
        if(mInstance == this) {
            //dealloc here

            base.OnDestroy();
        }
    }

    public override void SpawnFinish() {
        //start ai, player control, etc
        mCtrl.inputEnabled = true;
    }

    protected override void SpawnStart() {
        //initialize some things
    }

    protected override void Awake() {
        if(mInstance == null) {
            mInstance = this;

            base.Awake();

            //initialize variables
            mStats = GetComponent<Stats>();
            mCtrl = GetComponent<PlayerController>();
        }
        else
            DestroyImmediate(gameObject);
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)
    }
}

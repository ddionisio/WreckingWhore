using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
    public enum Type {
        HP,
        Power
    }

    public delegate void OnStatChange(Stats stat, Type which, float delta);

    public float maxHP;
    public float maxPower;
    public float defense;

    public float powerPerSec = 1.0f;

    public event OnStatChange statCallback;

    private float mCurHP;
    private float mCurPower;
    private bool mPowerRegenActive = false;

    public float hp {
        get { return mCurHP; }
        set {
            if(mCurHP != value) {
                float prev = mCurHP;
                mCurHP = Mathf.Clamp(value, 0.0f, maxHP); //allow overflow?

                if(statCallback != null)
                    statCallback(this, Type.HP, mCurHP - prev);
            }
        }
    }

    public float power {
        get { return mCurPower; }
        set {
            if(mCurPower != value) {
                float prev = mCurPower;
                mCurPower = Mathf.Clamp(value, 0.0f, maxPower); //allow overflow?

                if(statCallback != null)
                    statCallback(this, Type.Power, mCurPower - prev);
            }
        }
    }

    public bool powerRegenActive { get { return mPowerRegenActive; } set { mPowerRegenActive = value; } }

    /// <summary>
    /// Deal damage, a computation will determine amount of hitpoints removed
    /// </summary>
    public void Damage(float amt) {
        float dmg = Mathf.Clamp(amt - defense, 0, Mathf.Infinity);
        hp -= dmg;
    }

    public void Reset() {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            return;
#endif

        mCurHP = maxHP;
        mCurPower = maxPower;
    }

    void OnDestroy() {
        statCallback = null;
    }

    void Awake() {
        Reset();
    }

    void Update() {
        if(mPowerRegenActive) {
            power += powerPerSec * Time.deltaTime;
        }
    }
}

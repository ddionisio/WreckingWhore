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
    public float attack;
    public float defense;

    public bool powerDecay = false;
    public float powerDecayDelay;
    public float powerDecayPerSecond;

    public event OnStatChange statCallback;

    private float mCurHP;
    private float mCurPower;

    public float hp {
        get { return mCurHP; }
        set {
            if(mCurHP != value) {
                float prev = mCurHP;
                mCurHP = Mathf.Clamp(value, 0.0f, Mathf.Infinity); //allow overflow?

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
                mCurPower = Mathf.Clamp(value, 0.0f, Mathf.Infinity); //allow overflow?

                if(statCallback != null)
                    statCallback(this, Type.Power, mCurPower - prev);
            }
        }
    }

    /// <summary>
    /// Deal damage based on given attacker stats
    /// </summary>
    public void Damage(Stats attacker) {
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
}

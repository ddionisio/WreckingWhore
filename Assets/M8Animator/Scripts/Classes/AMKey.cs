using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Holoville.HOTween;

[AddComponentMenu("")]
public class AMKey : MonoBehaviour {
    public int version = 0; //for upgrading/initializing

    public int frame;
    public int easeType = (int)0;//AMTween.EaseType.linear; 			// ease type, AMTween.EaseType enum
    
    public float amplitude = 0.0f;
    public float period = 0.0f;

    public List<float> customEase = new List<float>();
    private AnimationCurve _cachedEaseCurve;
    public AnimationCurve easeCurve {
        get {
            if(_cachedEaseCurve == null || _cachedEaseCurve.keys.Length <= 0) _cachedEaseCurve = getCustomEaseCurve();
            return _cachedEaseCurve;
        }
    }

    public virtual void destroy() {
        Object.DestroyImmediate(this);
    }

    public virtual AMKey CreateClone() {
        Debug.LogError("Animator: No override for CreateClone()");
        return null;
    }

    /// <summary>
    /// Use sequence to insert callbacks, or some other crap, just don't insert the tweener you are returning!
    /// </summary>
    public virtual Tweener buildTweener(Sequence sequence, int frameRate) {
        Debug.LogError("Animator: No override for buildTweener.");
        return null;
    }

    public float getWaitTime(int frameRate, float delay) {
        return ((float)frame - 1f) / (float)frameRate - delay;
    }

    public virtual int getStartFrame() {
        return frame;
    }

    public virtual int getNumberOfFrames() {
        return 1;
    }

    public virtual AnimatorTimeline.JSONAction getJSONAction(int frameRate) {
        return null;
    }

    public void setCustomEase(AnimationCurve curve) {
        customEase = new List<float>();
        foreach(Keyframe k in curve.keys) {
            customEase.Add(k.time);
            customEase.Add(k.value);
            customEase.Add(k.inTangent);
            customEase.Add(k.outTangent);
        }
    }

    public AnimationCurve getCustomEaseCurve() {

        AnimationCurve curve = new AnimationCurve();
        if(customEase.Count < 0) {
            return curve;
        }
        if(customEase.Count % 4 != 0) {
            Debug.LogError("Animator: Error retrieving custom ease.");
            return curve;
        }
        for(int i = 0; i < customEase.Count; i += 4) {
            curve.AddKey(new Keyframe(customEase[i], customEase[i + 1], customEase[i + 2], customEase[i + 3]));
        }
        return curve;
    }

    public bool hasCustomEase() {
        if(easeType == (int)EaseType.AnimationCurve) return true;
        return false;
    }

    public bool setEaseType(int easeType) {
        if(easeType != this.easeType) {
            this.easeType = easeType;
            if(easeType == 32 && customEase.Count <= 0) {
                // set up default custom ease with linear
                customEase = new List<float>() {
					0f,0f,1f,1f,
					1f,1f,1f,1f
				};
            }
            return true;
        }
        return false;
    }
}

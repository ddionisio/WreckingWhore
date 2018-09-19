using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for playing sounds, need to inherit from this in order to allow global sound settings to affect.
/// Put this alongside an audio source
/// </summary>
[AddComponentMenu("M8/Audio/SoundPlayer")]
public class SoundPlayer : MonoBehaviour {
    public const float refRate = 44100.0f;

    /// <summary>
    /// Play the sound whenever it is enabled
    /// </summary>
    public bool playOnActive = false;

    public float playDelay = 0.0f;

    private bool mStarted = false;
    private float mDefaultVolume = 1.0f;

    public float defaultVolume { get { return mDefaultVolume; } set { mDefaultVolume = value; } }

    public virtual void Play() {
        UserSettings us = Main.instance.userSettings;
        GetComponent<AudioSource>().volume = mDefaultVolume * us.soundVolume;

        if(playDelay > 0.0f)
            GetComponent<AudioSource>().PlayDelayed(refRate * playDelay);
        else
            GetComponent<AudioSource>().Play();
    }

    protected virtual void OnEnable() {
        if(mStarted && playOnActive)
            Play();
    }

    protected virtual void Awake() {
        GetComponent<AudioSource>().playOnAwake = false;

        mDefaultVolume = GetComponent<AudioSource>().volume;
    }

    // Use this for initialization
    protected virtual void Start() {
        mStarted = true;

        if(playOnActive)
            Play();
    }

    void UserSettingsChanged(UserSettings us) {
        //if(audio.isPlaying)
        GetComponent<AudioSource>().volume = mDefaultVolume * us.soundVolume;    
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Audio/MusicManager")]
public class MusicManager : MonoBehaviour {
    public enum AutoPlayType {
        None,
        Order,
        Shuffled
    }

    [System.Serializable]
    public class MusicData {
        public string name;
        public AudioSource source;
        public float loopDelay = 0.0f;
        public bool loop = true;

        [System.NonSerialized]
        public float defaultVolume;
    }

    public delegate void OnMusicFinish(string curMusicName);

    public MusicData[] music;

    public float changeFadeOutDelay;

    public string playOnStart;

    public AutoPlayType autoPlay = AutoPlayType.None;

    public bool dontDestroy = false;
    public bool overrideExisting = false; //delete current available music manager

    public bool restartOnAppFocus;

    /// <summary>
    /// Callback when music is done playing.  Make sure the audio source 'loop' is set to false
    /// </summary>
    public event OnMusicFinish musicFinishCallback;

    private static MusicManager mInstance = null;

    private enum State {
        None,
        Playing,
        Changing
    }

    private const double rate = 44100;

    private Dictionary<string, MusicData> mMusic;
    private float mCurTime = 0;

    private State mState = State.None;

    private MusicData mCurMusic;
    private MusicData mNextMusic;

    private int mCurAutoplayInd = -1;

    public static MusicManager instance {
        get {
            return mInstance;
        }
    }

    public bool IsPlaying() {
        return mState == State.Playing;
    }

    public void Play(string name, bool immediate) {
        if(immediate) {
            Stop(false);
        }

        if(mCurMusic == null || immediate) {
            mCurMusic = mMusic[name];
            mCurMusic.source.volume = mCurMusic.defaultVolume * Main.instance.userSettings.musicVolume;
            mCurMusic.source.Play();
            SetState(State.Playing);
        }
        else {
            mNextMusic = mMusic[name];
            SetState(State.Changing);
        }

        //determine index for auto playlist
        if(autoPlay != AutoPlayType.None) {
            for(int i = 0; i < music.Length; i++) {
                if(music[i].name == name) {
                    mCurAutoplayInd = i;
                    break;
                }
            }
        }
    }

    public void Stop(bool fade) {
        if(mState != State.None) {
            if(fade) {
                mNextMusic = null;
                SetState(State.Changing);
            }
            else {
                mCurMusic.source.Stop();
                SetState(State.None);
            }
        }
    }

    private void AutoPlaylistNext() {
        Stop(false);

        mCurAutoplayInd++;
        if(mCurAutoplayInd >= music.Length)
            mCurAutoplayInd = 0;

        mCurMusic = music[mCurAutoplayInd];
        mCurMusic.source.volume = mCurMusic.defaultVolume * Main.instance.userSettings.musicVolume;
        mCurMusic.source.Play();
        SetState(State.Playing);
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;

        musicFinishCallback = null;
    }

    void Awake() {
        if(overrideExisting && mInstance != null) {
            mInstance.Stop(false);
            DestroyImmediate(mInstance.gameObject);
        }

        if(mInstance == null) {
            mInstance = this;

            mMusic = new Dictionary<string, MusicData>(music.Length);
            foreach(MusicData dat in music) {
                dat.defaultVolume = dat.source.volume;
                mMusic.Add(dat.name, dat);
            }

            if(autoPlay == AutoPlayType.Shuffled)
                M8.ArrayUtil.Shuffle(music);

            if(dontDestroy)
                Object.DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    // Use this for initialization
    void Start() {
        if(!string.IsNullOrEmpty(playOnStart)) {
            Play(playOnStart, true);
        }
        else if(autoPlay != AutoPlayType.None) {
            mCurAutoplayInd = -1;
            AutoPlaylistNext();
        }
    }

    void UserSettingsChanged(UserSettings us) {
        if(mCurMusic != null) {
            switch(mState) {
                case State.Playing:
                    mCurMusic.source.volume = mCurMusic.defaultVolume * us.musicVolume;
                    break;
            }
        }
    }

    void SetState(State state) {
        mState = state;
        mCurTime = 0;

        if(mState == State.None)
            mCurMusic = null;
    }

    // Update is called once per frame
    void Update() {
        switch(mState) {
            case State.None:
                break;
            case State.Playing:
                if(!(mCurMusic.source.loop || mCurMusic.source.isPlaying)) {
                    string curName = mCurMusic.name;

                    if(autoPlay != AutoPlayType.None)
                        AutoPlaylistNext();
                    else if(mCurMusic.loop) //loop
                        mCurMusic.source.Play((ulong)System.Math.Round(rate * ((double)mCurMusic.loopDelay)));
                    else {
                        SetState(State.None);
                    }

                    //callback
                    if(musicFinishCallback != null)
                        musicFinishCallback(curName);
                }
                break;
            case State.Changing:
                mCurTime += Time.deltaTime;
                if(mCurTime >= changeFadeOutDelay) {
                    mCurMusic.source.Stop();

                    if(mNextMusic != null) {
                        mCurMusic.source.volume = mCurMusic.defaultVolume * Main.instance.userSettings.musicVolume;
                        mNextMusic.source.Play();

                        mCurMusic = mNextMusic;
                        mNextMusic = null;

                        SetState(State.Playing);
                    }
                    else {
                        SetState(State.None);
                    }
                }
                else {
                    mCurMusic.source.volume = mCurMusic.defaultVolume * (1.0f - mCurTime / changeFadeOutDelay) * Main.instance.userSettings.musicVolume;
                }
                break;
        }
    }

    void OnApplicationFocus(bool focus) {
        if(restartOnAppFocus && focus && mState == State.Playing && mCurMusic != null) {
            mCurMusic.source.Stop();
            mCurMusic.source.Play();
        }
    }
}

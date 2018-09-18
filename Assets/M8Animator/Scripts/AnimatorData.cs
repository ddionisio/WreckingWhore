using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Holoville.HOTween;

[ExecuteInEditMode]
[AddComponentMenu("M8/Animator")]
public class AnimatorData : MonoBehaviour {
    public enum DisableAction {
        None,
        Pause,
        Stop
    }

    // show
    public List<AMTake> takes = new List<AMTake>();
    public AMTake playOnStart = null;

    public bool sequenceLoadAll = true;
    public bool sequenceKillWhenDone = false;

    public bool playOnEnable = false;

    public DisableAction onDisableAction = DisableAction.Pause;

    public UpdateType updateType = UpdateType.Update;
    // hide

    public bool isPlaying {
        get {
            if(nowPlayingTake == null) return false;

            Sequence seq = nowPlayingTake.sequence;

            return seq != null && !(seq.isPaused || seq.isComplete);
        }
    }

    public bool isPaused {
        get {
            return nowPlayingTake != null && nowPlayingTake.sequence != null && nowPlayingTake.sequence.isPaused;
        }
    }

    public bool isReversed {
        set {
            if(nowPlayingTake != null && nowPlayingTake.sequence != null) {
                if(value) {
                    if(!nowPlayingTake.sequence.isReversed)
                        nowPlayingTake.sequence.Reverse();
                }
                else {
                    if(nowPlayingTake.sequence.isReversed)
                        nowPlayingTake.sequence.Reverse();
                }
            }
        }

        get {
            return nowPlayingTake != null && nowPlayingTake.sequence != null && nowPlayingTake.sequence.isReversed;
        }
    }

    public string takeName {
        get {
            if(nowPlayingTake != null) return nowPlayingTake.name;
            return null;
        }
    }

    public float runningTime {
        get {
            if(takeName == null) return 0f;
            else {
                if(nowPlayingTake != null && nowPlayingTake.sequence != null)
                    return nowPlayingTake.sequence.elapsed;
                else
                    return 0.0f;
            }
        }
    }
    public float totalTime {
        get {
            if(takeName == null) return 0f;
            else return (float)nowPlayingTake.numFrames / (float)nowPlayingTake.frameRate;
        }
    }

    [System.NonSerialized]
    public bool isAnimatorOpen = false;
    [System.NonSerialized]
    public bool isInspectorOpen = false;
    [System.NonSerialized]
    public bool inPlayMode = false;
    [HideInInspector]
    public float zoom = 0.4f;
    [HideInInspector]
    public int currentTake;
    [HideInInspector]
    public int codeLanguage = 0; 	// 0 = C#, 1 = Javascript
    [HideInInspector]
    public float gizmo_size = 0.05f;
    [HideInInspector]
    public float width_track = 150f;
    // temporary variables for selecting a property
    //[HideInInspector] public bool didSelectProperty = false;
    //[HideInInspector] public AMPropertyTrack propertySelectTrack;
    //[HideInInspector] public Component propertyComponent;
    //[HideInInspector] public PropertyInfo propertyInfo;
    //[HideInInspector] public FieldInfo fieldInfo;
    [HideInInspector]
    public bool autoKey = false;

    [HideInInspector]
    [SerializeField]
    private GameObject _dataHolder;

    //[HideInInspector]
    //public float elapsedTime = 0f;
    // private
    private AMTake nowPlayingTake = null;
    //private bool isLooping = false;
    //private float takeTime = 0f;
    private bool mStarted = false;

    private int _prevTake = -1;

    public AMTake currentPlayingTake { get { return nowPlayingTake; } }

    public int prevTake { get { return _prevTake; } }

    public GameObject dataHolder {
        get {
            if(_dataHolder == null) {
                foreach(Transform child in transform) {
                    if(child.gameObject.name == "_animdata") {
                        _dataHolder = child.gameObject;
                        break;
                    }
                }

                if(_dataHolder) {
                    //refresh data?
                }
                else {
                    _dataHolder = new GameObject("_animdata");
                    _dataHolder.transform.parent = transform;
                    _dataHolder.SetActive(false);
                }
            }

            return _dataHolder;
        }
    }

    public object Invoker(object[] args) {
        switch((int)args[0]) {
            // check if is playing
            case 0:
                return isPlaying;
            // get take name
            case 1:
                return takeName;
            // play
            case 2:
                Play((string)args[1], true, 0f, (bool)args[2]);
                break;
            // stop
            case 3:
                Stop();
                break;
            // pause
            case 4:
                Pause();
                break;
            // resume
            case 5:
                Resume();
                break;
            // play from time
            case 6:
                Play((string)args[1], false, (float)args[2], (bool)args[3]);
                break;
            // play from frame
            case 7:
                Play((string)args[1], true, (float)((int)args[2]), (bool)args[3]);
                break;
            // preview frame
            case 8:
                PreviewValue((string)args[1], true, (float)args[2]);
                break;
            // preview time
            case 9:
                PreviewValue((string)args[1], false, (float)args[2]);
                break;
            // running time
            case 10:
                return runningTime;
            // total time
            case 11:
                if(takeName == null) return 0f;
                else return (float)nowPlayingTake.numFrames / (float)nowPlayingTake.frameRate;
            case 12:
                return isPaused;
            default:
                break;
        }
        return null;
    }

    void OnDestroy() {
        if(!Application.isPlaying) {
            if(_dataHolder) {
                DestroyImmediate(_dataHolder);
                _dataHolder = null;
            }
        }
        else {
            foreach(AMTake take in takes) {
                take.destroy();
            }
        }

        /*playOnStart = null;

        foreach(AMTake take in takes) {
            take.destroy();
        }

        takes.Clear();*/
    }

    void OnEnable() {
        if(mStarted) {
            if(playOnEnable) {
                if(nowPlayingTake == null && playOnStart != null)
                    Play(playOnStart.name, true, 0f, false);
                else
                    Resume();
            }
            //else if(playOnStart) {
            //Play(playOnStart.name, true, 0f, false);
            //}
        }
    }

    void OnDisable() {
        switch(onDisableAction) {
            case DisableAction.Pause:
                Pause();
                break;
            case DisableAction.Stop:
                Stop();
                break;
        }
    }

    void Start() {
        if(!Application.isPlaying)
            return;

        mStarted = true;
        if(sequenceLoadAll && takes != null) {
            foreach(AMTake take in takes)
                take.BuildSequence(gameObject.name, sequenceKillWhenDone, updateType);
        }

        if(playOnStart) {
            Play(playOnStart.name, true, 0.0f, false);
        }
    }

    void OnDrawGizmos() {
        if(!isAnimatorOpen) return;
        takes[currentTake].drawGizmos(gizmo_size, inPlayMode);
    }

    /*void Update() {
        if(_isPaused || nowPlayingTake == null) return;
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= takeTime) {
            nowPlayingTake.stopAudio();
            if(isLooping) Execute(nowPlayingTake);
            else nowPlayingTake = null;
        }
    }*/

    public string[] GenerateTakeNames(bool firstIndexNone = true) {
        if(takes != null) {
            string[] ret = firstIndexNone ? new string[takes.Count + 1] : new string[takes.Count];

            if(firstIndexNone)
                ret[0] = "None";

            for(int i = 0; i < takes.Count; i++) {
                ret[firstIndexNone ? i + 1 : i] = takes[i].name;
            }

            return ret;
        }
        else {
            return firstIndexNone ? new string[] { "None" } : null;
        }
    }

    public void PlayDefault(bool loop = false) {
        if(playOnStart) {
            Play(playOnStart.name, loop);
        }
    }

    // play take by name
    public void Play(string takeName, bool loop = false) {
        Play(takeName, true, 0f, loop);
    }

    public void PlayAtTime(string takeName, float time, bool loop = false) {
        Play(takeName, false, time, loop);
    }

    public void Pause() {
        if(nowPlayingTake == null) return;
        nowPlayingTake.stopAudio();
        //AMTween.Pause();

        if(nowPlayingTake.sequence != null)
            nowPlayingTake.sequence.Pause();

    }

    public void Resume() {
        if(nowPlayingTake == null) return;

        if(nowPlayingTake.sequence != null)
            nowPlayingTake.sequence.Play();
    }

    public void Stop() {
        if(nowPlayingTake == null) return;
        nowPlayingTake.stopAudio();
        nowPlayingTake.stopAnimations();

        if(nowPlayingTake.sequence != null) {
            nowPlayingTake.sequence.Pause();
            nowPlayingTake.sequence.GoTo(0);
        }

        nowPlayingTake = null;
    }

    public void GotoFrame(float frame) {
        if(nowPlayingTake != null && nowPlayingTake.sequence != null) {
            float t = frame / nowPlayingTake.frameRate;
            nowPlayingTake.sequence.GoTo(t);
        }
        else {
            Debug.LogWarning("No take playing...");
        }
    }

    public void Reverse() {
        if(nowPlayingTake == null) return;

        if(nowPlayingTake.sequence != null)
            nowPlayingTake.sequence.Reverse();
    }

    // play take by name from time
    public void PlayFromTime(string takeName, float time, bool loop = false) {
        Play(takeName, false, time, loop);
    }

    // play take by name from frame
    public void PlayFromFrame(string takeName, float frame, bool loop = false) {
        Play(takeName, true, frame, loop);
    }

    // preview a single frame (used for scrubbing)
    public void PreviewFrame(string takeName, float frame) {
        PreviewValue(takeName, true, frame);
    }

    // preview a single time (used for scrubbing)
    public void PreviewTime(string takeName, float time) {
        PreviewValue(takeName, false, time);
    }

    /// <summary>
    /// Return true if there are no nulls in references
    /// </summary>
    public bool CheckIntegrity() {
        int numTracks = 0;
        int numKeys = 0;

        foreach(AMTake take in takes) {
            if(take) {
                if(!take.CheckNulls(ref numKeys))
                    return false;

                numTracks += take.trackValues.Count;
            }
            else {
                return false;
            }
        }

        //check if actual takes in game object is the same count...
        GameObject dh = dataHolder;

        if(takes.Count != dh.GetComponentsInChildren<AMTake>(true).Length)
            return false;
        else if(numTracks != dh.GetComponentsInChildren<AMTrack>(true).Length)
            return false;
        else if(numKeys != dh.GetComponentsInChildren<AMKey>(true).Length)
            return false;

        return true;
    }

    void Play(string take_name, bool isFrame, float value, bool loop) {
        AMTake newPlayTake = getTake(take_name);

        if(!newPlayTake) {
            Stop();
            return;
        }

        if(newPlayTake != nowPlayingTake && nowPlayingTake != null) {
            Pause();
        }

        nowPlayingTake = newPlayTake;

        float startTime = value;
        if(isFrame) startTime /= nowPlayingTake.frameRate;

        float startFrame = 0;//isFrame ? value : nowPlayingTake.frameRate * value;

        if(nowPlayingTake.sequence == null)
            nowPlayingTake.BuildSequence(gameObject.name, sequenceKillWhenDone, updateType, startFrame);
        else {
            //TODO: make this more efficient
            if(value == 0.0f)
                nowPlayingTake.previewFrame(0, false, true);
            /*if(startTime > nowPlayingTake.sequence.duration)
                startFrame = (startTime / nowPlayingTake.sequence.duration) * nowPlayingTake.frameRate;

            nowPlayingTake.previewFrame(startFrame, false, true);*/
        }

        if(nowPlayingTake.sequence != null) {
            if(loop) {
                nowPlayingTake.sequence.loops = -1;
            }
            else {
                nowPlayingTake.sequence.loops = nowPlayingTake.numLoop;
            }

            nowPlayingTake.sequence.GoTo(startTime);
            nowPlayingTake.sequence.Play();
        }

        //isLooping = loop;

        //Execute(nowPlayingTake, isFrame, value);
    }

    void PreviewValue(string take_name, bool isFrame, float value) {
        AMTake take;
        if(nowPlayingTake && nowPlayingTake.name == takeName) take = nowPlayingTake;
        else take = getTake(take_name);
        if(!take) return;
        float startFrame = value;
        if(!isFrame) startFrame *= take.frameRate;	// convert time to frame
        take.previewFrame(startFrame);
    }

    void Execute(AMTake take, bool isFrame = true, float value = 0f /* frame or time */) {
        //if(nowPlayingTake != null)
        //AMTween.Stop();
        // delete AMCameraFade
        float startFrame = value;
        float startTime = value;
        if(!isFrame) startFrame *= take.frameRate;	// convert time to frame
        if(isFrame) startTime /= take.frameRate;	// convert frame to time
        take.executeActions(startFrame);
        //elapsedTime = startTime;
        //takeTime = (float)take.numFrames / (float)take.frameRate;
        nowPlayingTake = take;

    }

    public int getTakeCount() {
        return takes.Count;
    }

    public bool setCurrentTakeValue(int _take) {
        if(_take != currentTake) {
            _prevTake = currentTake;

            // reset preview to frame 1
            getCurrentTake().previewFrame(1f);
            // change take
            currentTake = _take;
            return true;
        }
        return false;
    }

    public AMTake getCurrentTake() {
        if(takes == null || currentTake >= takes.Count || currentTake < 0) return null;
        return takes[currentTake];
    }

    public AMTake getPreviousTake() {
        return takes != null && _prevTake >= 0 && _prevTake < takes.Count ? takes[_prevTake] : null;
    }

    public AMTake getTake(string takeName) {
        foreach(AMTake take in takes) {
            if(take.name == takeName) return take;
        }
        Debug.LogError("Animator: Take '" + takeName + "' not found.");
        return new AMTake(null);
    }

    public AMTake addTake() {
        string name = "Take" + (takes.Count + 1);
        AMTake a = AMTake.NewInstance(dataHolder);
        // set defaults
        a.name = name;
        makeTakeNameUnique(a);
        a.numLoop = 1;
        a.loopMode = LoopType.Restart;
        a.frameRate = 24;
        a.numFrames = 1440;
        a.startFrame = 1;
        a.selectedFrame = 1;
        a.selectedTrack = -1;
        a.playbackSpeedIndex = 2;
        //a.lsTracks = new List<AMTrack>();
        //a.dictTracks = new Dictionary<int,AMTrack>();
        a.trackValues = new List<AMTrack>();
        takes.Add(a);
        selectTake(takes.Count - 1);
        return a;
    }

    /// <summary>
    /// This will only duplicate the tracks and groups
    /// </summary>
    /// <param name="take"></param>
    public List<UnityEngine.Object> duplicateTake(AMTake dupTake) {
        List<UnityEngine.Object> ret = new List<Object>();

        AMTake a = AMTake.NewInstance(dataHolder);

        ret.Add(a);

        a.name = dupTake.name;
        makeTakeNameUnique(a);
        a.numLoop = dupTake.numLoop;
        a.loopMode = dupTake.loopMode;
        a.frameRate = dupTake.frameRate;
        a.numFrames = dupTake.numFrames;
        a.startFrame = dupTake.startFrame;
        a.selectedFrame = 1;
        a.selectedTrack = dupTake.selectedTrack;
        a.selectedGroup = dupTake.selectedGroup;
        a.playbackSpeedIndex = 2;
        //a.lsTracks = new List<AMTrack>();
        //a.dictTracks = new Dictionary<int,AMTrack>();

        if(dupTake.rootGroup != null) {
            a.rootGroup = dupTake.rootGroup.duplicate();
        }
        else {
            a.initGroups();
        }

        a.group_count = dupTake.group_count;

        if(dupTake.groupValues != null) {
            a.groupValues = new List<AMGroup>();
            foreach(AMGroup grp in dupTake.groupValues) {
                a.groupValues.Add(grp.duplicate());
            }
        }

        a.track_count = dupTake.track_count;

        if(dupTake.trackValues != null) {
            a.trackValues = new List<AMTrack>();
            foreach(AMTrack track in dupTake.trackValues) {
                AMTrack dupTrack = track.duplicate(a);

                a.trackValues.Add(dupTrack);

                ret.Add(dupTrack);
            }
        }
        a.contextSelection = new List<int>();
        a.ghostSelection = new List<int>();
        a.contextSelectionTracks = new List<int>();

        takes.Add(a);
        selectTake(takes.Count - 1);

        return ret;
    }

    public void deleteTake(int index) {
        //if(shouldCheckDependencies) shouldCheckDependencies = false;
        if(playOnStart == takes[index]) playOnStart = null;
        takes[index].destroy();
        takes.RemoveAt(index);
        if((currentTake >= index) && (currentTake > 0)) currentTake--;
    }

    public void deleteCurrentTake() {
        deleteTake(currentTake);
    }

    public void selectTake(int index) {
        if(currentTake != index)
            _prevTake = currentTake;

        currentTake = index;
    }

    public void selectTake(string name) {
        for(int i = 0; i < takes.Count; i++)
            if(takes[i].name == name) {
                selectTake(i);
                break;
            }
    }
    public void makeTakeNameUnique(AMTake take) {
        bool loop = false;
        int count = 0;
        do {
            if(loop) loop = false;
            foreach(AMTake _take in takes) {
                if(_take != take && _take.name == take.name) {
                    if(count > 0) take.name = take.name.Substring(0, take.name.Length - 3);
                    count++;
                    take.name += "(" + count + ")";
                    loop = true;
                    break;
                }
            }
        } while(loop);
    }

    public string[] getTakeNames() {
        string[] names = new string[takes.Count + 2];
        for(int i = 0; i < takes.Count; i++) {
            names[i] = takes[i].name;
        }
        names[names.Length - 2] = "Create new...";
        names[names.Length - 1] = "Duplicate current...";
        return names;
    }

    public int getTakeIndex(AMTake take) {
        for(int i = 0; i < takes.Count; i++) {
            if(takes[i] == take) return i;
        }
        return -1;
    }
    public bool setCodeLanguage(int codeLanguage) {
        if(this.codeLanguage != codeLanguage) {
            this.codeLanguage = codeLanguage;
            return true;
        }
        return false;
    }
    public bool setGizmoSize(float gizmo_size) {
        if(this.gizmo_size != gizmo_size) {
            this.gizmo_size = gizmo_size;
            // update target gizmo size
            foreach(Object target in GameObject.FindObjectsOfType(typeof(AMTarget))) {
                if((target as AMTarget).gizmo_size != gizmo_size) (target as AMTarget).gizmo_size = gizmo_size;
            }
            return true;
        }
        return false;
    }

    /*public bool setShowWarningForLostReferences(bool showWarningForLostReferences) {
        if(this.showWarningForLostReferences != showWarningForLostReferences) {
            this.showWarningForLostReferences = showWarningForLostReferences;
            return true;
        }
        return false;
    }*/

    public void deleteAllTakesExcept(AMTake take) {
        for(int index = 0; index < takes.Count; index++) {
            if(takes[index] == take) continue;
            deleteTake(index);
            index--;
        }
    }

    public void mergeWith(AnimatorData _aData) {
        foreach(AMTake take in _aData.takes) {
            takes.Add(take);
            makeTakeNameUnique(take);
        }
    }

    public List<GameObject> getDependencies(AMTake _take = null) {
        // if only one take
        if(_take != null) return _take.getDependencies().ToList();

        // if all takes
        List<GameObject> ls = new List<GameObject>();
        foreach(AMTake take in takes) {
            ls = ls.Union(take.getDependencies()).ToList();
        }
        return ls;
    }

    public List<GameObject> updateDependencies(List<GameObject> newReferences, List<GameObject> oldReferences) {
        List<GameObject> lsFlagToKeep = new List<GameObject>();
        foreach(AMTake take in takes) {
            lsFlagToKeep = lsFlagToKeep.Union(take.updateDependencies(newReferences, oldReferences)).ToList();
        }
        return lsFlagToKeep;
    }

}

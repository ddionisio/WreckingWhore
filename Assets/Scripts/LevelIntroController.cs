using UnityEngine;
using System.Collections;

public class LevelIntroController : MonoBehaviour {
    public UILabel label;
    public GameObject[] levelHighlights;
    public float inputDelay = 1.0f;
    public float delay = 3.0f;

    private string mToLevel;
    private bool mDone;
    private bool mInputReady;

    void Awake() {

    }

    // Use this for initialization
    void Start() {
        mDone = false;
        mInputReady = false;

        foreach(GameObject go in levelHighlights) {
            go.SetActive(false);
        }

        int level = SceneState.instance.GetGlobalValue(SceneManager.levelString);
        int levelInd = level - 1;

        if(levelInd == levelHighlights.Length - 1) {
            mToLevel = "boss";
        }
        else {
            mToLevel = SceneManager.levelString + level;
        }

        label.text = GameLocalize.GetText(mToLevel);

        levelHighlights[levelInd].SetActive(true);

        Invoke("DoInput", inputDelay);
        Invoke("DoFinish", delay);
    }

    void DoFinish() {
        Main.instance.sceneManager.LoadScene(mToLevel);
        mDone = true;
    }

    void DoInput() {
        mInputReady = true;
    }

    void Update() {
        if(!mDone && mInputReady) {
            if(Input.anyKeyDown)
                DoFinish();
        }
    }
}

using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public Bounds levelBounds {
        get {
            float boundW = mTileMap.width * mTileMap.data.tileSize.x;
            float boundH = mTileMap.height * mTileMap.data.tileSize.y;

            return new Bounds(
                new Vector3(mTileMap.data.tileOrigin.x + boundW * 0.5f, mTileMap.data.tileOrigin.y + boundH * 0.5f),
                new Vector3(boundW, boundH));
        }
    }

    private static LevelManager mInstance;

    private tk2dTileMap mTileMap;

    public static LevelManager instance { get { return mInstance; } }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;

        GameObject levelGo = GameObject.FindGameObjectWithTag("Level");
        if(levelGo) {
            mTileMap = levelGo.GetComponentInChildren<tk2dTileMap>();
        }
        else {
            Debug.LogError("Level not found!!!");
        }
    }
}

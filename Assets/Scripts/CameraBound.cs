using UnityEngine;
using System.Collections;

public class CameraBound : MonoBehaviour {
    public tk2dCamera mainCamera;

    [System.NonSerialized]
    public Bounds bounds;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(bounds.size.x > 0.0f && bounds.size.y > 0.0f) {
            Vector3 pos = transform.position;

            Rect screen = mainCamera.ScreenExtents;

            if(pos.x - screen.width * 0.5f < bounds.min.x)
                pos.x = bounds.min.x + screen.width * 0.5f;
            else if(pos.x + screen.width * 0.5f > bounds.max.x)
                pos.x = bounds.max.x - screen.width * 0.5f;

            if(pos.y - screen.height * 0.5f < bounds.min.y)
                pos.y = bounds.min.y + screen.height * 0.5f;
            else if(pos.y + screen.height * 0.5f > bounds.max.y)
                pos.y = bounds.max.y - screen.height * 0.5f;

            transform.position = pos;
        }
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Camera/ForceOrthoSort")]
[ExecuteInEditMode]
public class CameraForceOrthoSort : MonoBehaviour {

    void OnEnable() {
        GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
    }

    void OnPreCull() {
        GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
    }

#if UNITY_EDITOR
    void LateUpdate() {
        if(!Application.isPlaying) {
            GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
        }
    }
#endif
}

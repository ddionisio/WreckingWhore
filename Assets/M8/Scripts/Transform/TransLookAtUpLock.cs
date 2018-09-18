using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/LookAtUpLock")]
public class TransLookAtUpLock : MonoBehaviour {
    public string targetTag = "MainCamera"; //if target is null
    public Transform target;

    public bool visibleCheck = true; //if true, only compute if source's renderer is visible
    public bool backwards;

    public Transform source; //if null, use this transform

    void Awake() {
        if(target == null) {
            GameObject go = GameObject.FindGameObjectWithTag(targetTag);
            if(go != null)
                target = go.transform;
        }

        if(source == null)
            source = transform;
    }
    
    // Update is called once per frame
    void Update() {
        if(!visibleCheck || source.renderer.isVisible) {
            float angle = M8.MathUtil.AngleForwardAxis(
                source.worldToLocalMatrix,
                source.position,
                backwards ? Vector3.back : Vector3.forward, 
                target.position);
            source.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}

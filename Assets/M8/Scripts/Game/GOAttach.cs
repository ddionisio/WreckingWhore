﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Make sure this is on an object with a rigidbody!
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("M8/Game Object/Attach")]
public class GOAttach : MonoBehaviour {
    public Transform target;
    public Vector3 offset;

    // Update is called once per frame
    void Update() {
        if(target != null) {
            if(GetComponent<Collider>() != null) {
                Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(GetComponent<Collider>().bounds.center);

                transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
            }
            else {
                transform.position = target.position + target.rotation * offset;
            }

            transform.rotation = target.rotation;
        }
    }
}

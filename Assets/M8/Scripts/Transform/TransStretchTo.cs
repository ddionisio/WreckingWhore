﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/StretchAndDirTo")]
public class TransStretchTo : MonoBehaviour {
    public enum Dir {
        Up,
        Right,
        Forward
    }

    public Transform anchor;
    public Transform target;

    public Dir dir;
    public float ofs;

    public bool anchorColliderCenter = true;
    public bool colliderCenter = true;

    public bool reverse;

    public bool lockX;
    public bool lockY;
    public bool lockZ = true; //true for 2d

    // Update is called once per frame
    void Update() {
        if(target) {
            Vector3 apos = anchor ? anchorColliderCenter && anchor.collider ? anchor.collider.bounds.center : anchor.position : transform.position;
            Vector3 pos = colliderCenter && target.collider ? target.collider.bounds.center : target.position;

            Vector3 d = pos - apos;

            if(lockX)
                d.x = 0;
            if(lockY)
                d.y = 0;
            if(lockZ)
                d.z = 0;

            float len = d.magnitude;
            if(len > 0) {
                d /= len;

                Vector3 s = transform.localScale;

                switch(dir) {
                    case Dir.Up:
                        transform.up = reverse ? -d : d;
                        s.y = len + ofs;
                        break;

                    case Dir.Right:
                        transform.right = reverse ? -d : d;
                        s.x = len + ofs;
                        break;

                    case Dir.Forward:
                        transform.forward = reverse ? -d : d;
                        s.z = len + ofs;
                        break;
                }

                transform.position = apos;
                transform.localScale = s;
            }
        }
    }
}

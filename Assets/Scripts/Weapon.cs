﻿using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {
    public enum Dir {
        Up,
        Down,
        Left,
        Right
    }

    public delegate void OnChange(Weapon weapon);

    public GameObject owner;

    public event OnChange finishCallback;

    protected Dir mDir = Dir.Right;
    protected bool mActive;

    public bool isActive { get { return mActive; } }

    public virtual void SetDir(Dir dir) {
        mDir = dir;
    }

    public virtual void Cancel() {
        mActive = false;
    }

    public virtual void Attack() {
        mActive = true;
    }

    public virtual void Finish() {
        mActive = false;

        if(finishCallback != null)
            finishCallback(this);
    }

    protected virtual void OnDestroy() {
        finishCallback = null;
    }
}

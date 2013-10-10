using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {
    public bool playOnEnable;

    private bool mStarted;

    public void Play() {
        particleSystem.Play();
    }

    public void Stop() {
        particleSystem.Stop();
    }

    public void Pause() {
        particleSystem.Pause();
    }

    public void SetLoop(bool loop) {
        particleSystem.loop = loop;
    }

    void OnEnable() {
        if(mStarted && playOnEnable)
            particleSystem.Play();
    }

    void Start() {
        mStarted = true;

        if(playOnEnable)
            particleSystem.Play();
    }
}

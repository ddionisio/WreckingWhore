using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {
    public bool playOnEnable;

    private bool mStarted;

    public void Play() {
        GetComponent<ParticleSystem>().Play();
    }

    public void Stop() {
        GetComponent<ParticleSystem>().Stop();
    }

    public void Pause() {
        GetComponent<ParticleSystem>().Pause();
    }

    public void SetLoop(bool loop) {
        GetComponent<ParticleSystem>().loop = loop;
    }

    void OnEnable() {
        if(mStarted && playOnEnable)
            GetComponent<ParticleSystem>().Play();
    }

    void Start() {
        mStarted = true;

        if(playOnEnable)
            GetComponent<ParticleSystem>().Play();
    }
}

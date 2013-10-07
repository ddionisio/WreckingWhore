using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public float damage;
    public bool knockback;

    public string[] hitTags;

    void OnTriggerStay(Collider col) {
        GameObject go = col.gameObject;

        bool isHit = false;
        for(int i = 0, max = hitTags.Length; i < max; i++) {
            if(hitTags[i] == go.tag) {
                isHit = true;
                break;
            }
        }

        if(isHit) {
            go.SendMessage("OnHit", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    // Use this for initialization
    void Start() {

    }
}

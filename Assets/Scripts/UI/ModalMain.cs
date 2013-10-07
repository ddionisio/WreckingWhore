using UnityEngine;
using System.Collections;

public class ModalMain : UIController {
    public UIEventListener start;

    protected override void OnActive(bool active) {
        if(active) {
            start.onClick = OnStart;
        }
        else {
            start.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnStart(GameObject go) {
        Main.instance.sceneManager.LoadScene("test");
    }
}

using UnityEngine;
using System.Collections;

public class ModalVictory : UIController {
    public UIEventListener yes;

    protected override void OnActive(bool active) {
        if(active) {
            yes.onClick = OnStart;
        }
        else {
            yes.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnStart(GameObject go) {
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }
}

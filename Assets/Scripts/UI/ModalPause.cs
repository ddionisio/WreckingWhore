using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
    public UIEventListener options;
    public UIEventListener exit;

    protected override void OnActive(bool active) {
        if(active) {
            options.onClick = OnOptions;
            exit.onClick = OnExit;
        }
        else {
            options.onClick = null;
            exit.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnOptions(GameObject go) {
        UIModalManager.instance.ModalOpen("options");
    }

    void OnExit(GameObject go) {
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }
}

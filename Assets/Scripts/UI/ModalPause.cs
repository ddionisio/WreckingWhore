using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
    public UIEventListener options;
    public UIEventListener help;
    public UIEventListener exit;

    protected override void OnActive(bool active) {
        if(active) {
            options.onClick = OnOptions;
            help.onClick = OnHelp;
            exit.onClick = OnExit;
        }
        else {
            options.onClick = null;
            help.onClick = null;
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

    void OnHelp(GameObject go) {
        UIModalManager.instance.ModalOpen("help");
    }

    void OnExit(GameObject go) {
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }
}

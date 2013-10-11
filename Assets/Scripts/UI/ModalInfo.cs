using UnityEngine;
using System.Collections;

public class ModalInfo : UIController {
    public UIEventListener stub;

    protected override void OnActive(bool active) {
        if(active) {
            stub.onClick = OnStart;
        }
        else {
            stub.onClick = null;
        }
    }

    protected override void OnOpen() {
        UICamera.selectedObject = stub.gameObject;
    }

    protected override void OnClose() {
    }

    void OnStart(GameObject go) {
        UIModalManager.instance.ModalCloseTop();
    }
}

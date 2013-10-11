using UnityEngine;
using System.Collections;

public class ModalPage : UIController {
    public string endScene;

    public NGUIPage page;

    protected override void OnActive(bool active) {
        InputManager input = Main.instance.input;

        if(active) {
            page.pageEndCallback += OnEnd;

            input.AddButtonCall(0, InputAction.MenuAccept, OnMenuAccept);
            input.AddButtonCall(0, InputAction.MenuCancel, OnMenuEscape);
        }
        else {
            page.pageEndCallback -= OnEnd;

            input.RemoveButtonCall(0, InputAction.MenuAccept, OnMenuAccept);
            input.RemoveButtonCall(0, InputAction.MenuCancel, OnMenuEscape);
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnMenuAccept(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed)
            page.NextPage();
    }

    void OnMenuEscape(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed)
            page.End();
    }

    void OnEnd() {
        Main.instance.sceneManager.LoadScene(endScene);
    }
}

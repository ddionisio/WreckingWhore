using UnityEngine;
using System.Collections;

public class ModalOptions : UIController {
    public UIEventListener music;
    public UIEventListener sound;

    public UILabel musicLabel;
    public UILabel soundLabel;

    void RefreshLabels() {
        musicLabel.text = string.Format("MUSIC: {0}", Main.instance.userSettings.musicVolume > 0.0f ? "ON" : "OFF");
        soundLabel.text = string.Format("SOUND: {0}", Main.instance.userSettings.soundVolume > 0.0f ? "ON" : "OFF");
    }

    protected override void OnActive(bool active) {
        if(active) {
            music.onClick = OnMusicClick;
            sound.onClick = OnSoundClick;
        }
        else {
            music.onClick = null;
            sound.onClick = null;
        }
    }

    protected override void OnOpen() {
        RefreshLabels();
    }

    protected override void OnClose() {
    }

    void OnSoundClick(GameObject go) {
        Main.instance.userSettings.soundVolume = Main.instance.userSettings.soundVolume > 0.0f ? 0.0f : 1.0f;

        Main.instance.userSettings.Save();

        RefreshLabels();
    }

    void OnMusicClick(GameObject go) {
        Main.instance.userSettings.musicVolume = Main.instance.userSettings.musicVolume > 0.0f ? 0.0f : 1.0f;

        Main.instance.userSettings.Save();

        RefreshLabels();
    }
}

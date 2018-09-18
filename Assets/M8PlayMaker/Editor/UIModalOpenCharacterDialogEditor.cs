using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;

namespace M8.PlayMaker {
    [CustomActionEditor(typeof(UIModalOpenCharacterDialog))]
    public class UIModalOpenCharacterDialogEditor : CustomActionEditor {
        private bool mChoiceFoldout = false;
        private int mSize = -1;

        public override bool OnGUI() {
            UIModalOpenCharacterDialog data = target as UIModalOpenCharacterDialog;
                        
            EditField("modalRef");
            EditField("isLocalize");
            EditField("closeOnAction");
            EditField("actionEvent");
            EditField("name");
            EditField("text");
            EditField("portrait");
            EditField("choiceOutput");

            M8.Editor.Utility.DrawSeparator();

            M8.Editor.Utility.DrawStringArray("Choices", ref mChoiceFoldout, ref data.choices, ref mSize);

            return GUI.changed;
        }
    }
}
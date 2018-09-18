using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;

namespace M8.PlayMaker {
    [CustomActionEditor(typeof(UIModalOpenCharacterDialogSequenceLocalized))]
    public class UIModalOpenCharacterDialogSequenceLocalizedEditor : CustomActionEditor {
        private bool mChoiceFoldout = false;
        private int mSize = -1;

        public override bool OnGUI() {
            UIModalOpenCharacterDialogSequenceLocalized data = target as UIModalOpenCharacterDialogSequenceLocalized;

            EditField("modalRef");
            EditField("closeOnAction");
            EditField("actionEvent");
            EditField("name");
            EditField("textPrefix");
            EditField("textIndexStart");
            EditField("textIndexEnd");
            EditField("portrait");
            EditField("choiceOutput");

            M8.Editor.Utility.DrawSeparator();

            M8.Editor.Utility.DrawStringArray("Choices", ref mChoiceFoldout, ref data.choices, ref mSize);

            return GUI.changed;
        }
    }
}
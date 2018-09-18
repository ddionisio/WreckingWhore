using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;

namespace M8.PlayMaker {
    [CustomActionEditor(typeof(UIModalOpenCharacterDialogMulti))]
    public class UIModalOpenCharacterDialogMultiEditor : CustomActionEditor {
        private bool mChoiceFoldout = false;
        private int mChoiceSize = -1;

        private bool mTextFoldout = false;
        private int mTextSize = -1;

        public override bool OnGUI() {
            UIModalOpenCharacterDialogMulti data = target as UIModalOpenCharacterDialogMulti;

            EditField("modalRef");
            EditField("isLocalize");
            EditField("closeOnAction");
            EditField("actionEvent");
            EditField("name");
            EditField("portrait");
            EditField("choiceOutput");

            M8.Editor.Utility.DrawSeparator();

            M8.Editor.Utility.DrawStringArray("Texts", ref mTextFoldout, ref data.texts, ref mTextSize);

            M8.Editor.Utility.DrawSeparator();

            M8.Editor.Utility.DrawStringArray("Choices", ref mChoiceFoldout, ref data.choices, ref mChoiceSize);

            return GUI.changed;
        }
    }
}
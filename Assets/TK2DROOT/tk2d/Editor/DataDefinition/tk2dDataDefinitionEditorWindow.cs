using UnityEngine;
using UnityEditor;
using System.Collections;

public class tk2dDataDefinitionEditorWindow : EditorWindow {
	tk2dDataDefinition[] dataDefinitions = new tk2dDataDefinition[0];
	Vector2 scrollBar = Vector2.zero;
	Vector2 textScrollBar = Vector2.zero;

	string logData = "";

	void OnEnable() {
		dataDefinitions = tk2dDataDefinitionBuilder.FindDefinitionsInProject();
	}

	void OnGUI() {
		EditorGUILayout.HelpBox("The data definition feature is in beta and is unsupported.", MessageType.Warning);

		EditorGUILayout.BeginVertical( "box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		scrollBar = EditorGUILayout.BeginScrollView(scrollBar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

		foreach (tk2dDataDefinition def in dataDefinitions) {
			GUILayout.BeginHorizontal();

			GUILayout.Label(def.GetType().ToString());

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Build")) {
				logData = tk2dDataDefinitionBuilder.Build(new tk2dDataDefinition[] { def });
				dataDefinitions = tk2dDataDefinitionBuilder.FindDefinitionsInProject();
			}
			GUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		if (logData.Length > 0) {
			textScrollBar = EditorGUILayout.BeginScrollView(textScrollBar, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
			GUILayout.TextArea(logData);
			EditorGUILayout.EndScrollView();
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Build All")) {
			logData = tk2dDataDefinitionBuilder.Build(dataDefinitions);
			dataDefinitions = tk2dDataDefinitionBuilder.FindDefinitionsInProject();
		}
		EditorGUILayout.EndHorizontal();
	}

	[UnityEditor.MenuItem(tk2dMenu.root + "Data Definition Editor", false, 2)]
	static void ShowDataDefinitionEditor() {
		GetWindow<tk2dDataDefinitionEditorWindow>();
	}
}

﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SceneSerializerSpawnItem))]
public class SceneSerializerSpawnItemInspector : Editor {
    public override void OnInspectorGUI() {
        SceneSerializerSpawnItem data = target as SceneSerializerSpawnItem;

        if(Application.isPlaying) {
            EditorGUILayout.LabelField("id", data.id.ToString());
        }
        else {
            EditorGUILayout.LabelField("id", "--");
        }
    }
}

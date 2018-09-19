#define ENABLE_UNLOAD_MANAGER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class tk2dEditorSpriteDataUnloader : MonoBehaviour {

	public static void Register(tk2dSpriteCollectionData data) {
#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR
		GetInst();
#endif
	}

	public static void Unregister(tk2dSpriteCollectionData data) {
#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR
		GetInst();
#endif
	}

#if ENABLE_UNLOAD_MANAGER && UNITY_EDITOR

	static tk2dEditorSpriteDataUnloader _inst = null;	
	static tk2dEditorSpriteDataUnloader GetInst() {
		if (_inst == null) {
			tk2dEditorSpriteDataUnloader[] allInsts = Resources.FindObjectsOfTypeAll(typeof(tk2dEditorSpriteDataUnloader)) as tk2dEditorSpriteDataUnloader[];
			_inst = (allInsts.Length > 0) ? allInsts[0] : null;
			if (_inst == null) {
				GameObject go = new GameObject("@tk2dEditorSpriteDataUnloader");
				go.hideFlags = HideFlags.HideAndDontSave;
				_inst = go.AddComponent<tk2dEditorSpriteDataUnloader>();
			}
		}
		return _inst;
	}



	void OnEnable() {
		UnityEditor.EditorApplication.update += EditorUpdate;
	}

	void OnDisable() {
		UnityEngine.Object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)) as UnityEngine.Object[];
		DestroyInternalObjects( allObjects );		
		UnityEditor.EditorApplication.update -= EditorUpdate;
	}

	void DestroyInternalObjects(UnityEngine.Object[] allObjects) {
		foreach (UnityEngine.Object obj in allObjects) { 
			if (obj.name.IndexOf(tk2dSpriteCollectionData.internalResourcePrefix) == 0)  {
				Object.DestroyImmediate(obj); 
			}
		}
	}

	public void DestroyDisconnectedResources() {
		List<UnityEngine.Object> allObjects = new List<UnityEngine.Object>( Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)) as UnityEngine.Object[] );
		tk2dSpriteCollectionData[] objects = Resources.FindObjectsOfTypeAll(typeof(tk2dSpriteCollectionData)) as tk2dSpriteCollectionData[];
		foreach (tk2dSpriteCollectionData data in objects) {
			if (data.needMaterialInstance) {
				if (data.textureInsts != null) {
					foreach (Texture2D tex in data.textureInsts) {
						if (allObjects.Contains(tex)) {
							allObjects.Remove(tex);
						}
					}
				}
				if (data.materialInsts != null) {
					foreach (Material mtl in data.materialInsts) {
						if (allObjects.Contains(mtl)) {
							allObjects.Remove(mtl);
						}
					}
				}
			}
		}
		DestroyInternalObjects( allObjects.ToArray() );
	}

	public string oldScene = "";
	void EditorUpdate() {
		if (oldScene != UnityEditor.EditorApplication.currentScene) {
			oldScene = UnityEditor.EditorApplication.currentScene;
			DestroyDisconnectedResources();
		}
	}

#endif
}

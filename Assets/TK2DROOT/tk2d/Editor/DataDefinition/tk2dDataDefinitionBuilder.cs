using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Exposing new features to the data definition system.
// ----------------------------------------------------
// 1. Find the appropriate tk2dDDLNS class in tk2dDataDefiniton
// 2. Add the public function for the property, remember to "return this" to follow convention.
// 3. The public function should set to a backing variable. eg. Sprite.Anchor(...) writes to _anchor
// 4. Be sure to use the appropriate function overloads instead of default values, MonoDevelop doesn't like it.
// 5. Look for the appropriate Fill* function in this file, and copy the values to the sprite collection

class tk2dDataDefinitionAnimationBuilder {

	Dictionary<string, tk2dSpriteCollectionData> spriteCollectionLUT = new Dictionary<string, tk2dSpriteCollectionData>();
	Dictionary<string, tk2dDDLNS.SpriteCollection> allCollections = new Dictionary<string, tk2dDDLNS.SpriteCollection>(); 
	Dictionary<string, tk2dDDLNS.SpriteCollection> allSprites = new Dictionary<string, tk2dDDLNS.SpriteCollection>();
	List<string> ambiguousSpriteNames = new List<string>();

	public tk2dDataDefinitionAnimationBuilder(List<tk2dDDLNS.SpriteCollection> collections) {
		// Build a map of all sprites
		foreach (tk2dDDLNS.SpriteCollection sc in collections) {
			string scn = sc._name.ToLower();
			if (allCollections.ContainsKey(scn)) {
				throw new System.Exception("Duplicate sprite collection name");
			}
			allCollections[scn] = sc;

			foreach (tk2dDDLNS.Sprite s in sc._sprites) {
				string n = s._spriteName.ToLower();
				if (allSprites.ContainsKey(n)) {
					ambiguousSpriteNames.Add(n);					
				}
				else {
					allSprites[n] = sc;
				}
			}
		}
	}

	public void FinalizeAnimationWildcards(tk2dDDLNS.AnimationCollection anim) {
		foreach (tk2dDDLNS.Clip clip in anim._clips) {
			List<tk2dDDLNS.Frame> frames = new List<tk2dDDLNS.Frame>();
			foreach (tk2dDDLNS.Frame frame in clip._frames) {

				// Wildcard
				if (frame._name.Contains("?") || frame._name.Contains("*")) {
					List<string> frameNames = new List<string>();
					Regex regex = new Regex("^" + Regex.Escape( frame._name.ToLower() ).Replace(@"\*", "(.*)").Replace(@"\?", "(.)") + "$");

					// Collection name not defined, match from global sprite list
					if (frame._collectionName == null) {
						foreach (string s in allSprites.Keys) {
							if (regex.IsMatch(s)) {

								if (frame._rangeMin >= 0 && frame._rangeMax >= 0) {
									Match match = regex.Match(s);
									if (match.Groups.Count > 1) {
										int num = 0;
										if (int.TryParse( match.Groups[1].ToString(), out num ) && num >= frame._rangeMin && num <= frame._rangeMax) {
											frameNames.Add(s);
										}
									}
								}
								else {
									frameNames.Add(s);
								}
							}
						}
					}
					else {
						string cname = frame._collectionName.ToLower();
						if (!allCollections.ContainsKey(cname)) {
							throw new System.Exception(string.Format("Can't find collection {0} for clip {1}", cname, clip._name));
						}
						tk2dDDLNS.SpriteCollection collection = allCollections[cname];
						foreach (tk2dDDLNS.Sprite sprite in collection._sprites) {
							string name = sprite._spriteName.ToLower();
							if (regex.IsMatch(name)) {
								frameNames.Add(name);
							}
						}
					}

					// All matching framenames
					if (frameNames.Count == 0) {
						throw new System.Exception(string.Format("{0} for clip '{1}' did not match any sprite names", frame._name, clip._name));
					}

					// Sort
					frameNames.Sort(new tk2dEditor.Shared.NaturalComparer());
					if (frame._reverse) {
						frameNames.Reverse();
					}

					// Add to list
					foreach (string s in frameNames) {
						tk2dDDLNS.Frame f = frame._Copy;
						f._name = s;
						f._collectionName = allSprites[s]._name.ToLower();
						frames.Add(f);
					}
				}
				else {
					string nameKey = frame._name.ToLower();
					if (frame._collectionName == null) {
						if (!allSprites.ContainsKey(nameKey)) {
							throw new System.Exception(string.Format("Can't find sprite {0} for clip {1}", frame._name, clip._name));
						}
						frame._name = nameKey;
						frame._collectionName = allSprites[nameKey]._name.ToLower();
					}
					else {
						string cname = frame._collectionName.ToLower();
						frame._collectionName = cname;
						if (!allCollections.ContainsKey(cname)) {
							throw new System.Exception(string.Format("Can't find collection {0} for clip {1}", cname, clip._name));
						}
						tk2dDDLNS.SpriteCollection collection = allCollections[cname];
						bool found = false;
						foreach (tk2dDDLNS.Sprite sprite in collection._sprites) {
							string name = sprite._spriteName.ToLower();
							if (name == nameKey) {
								frame._name = nameKey;
								found = true;
								break;
							}
						}
						if (!found) {
							throw new System.Exception(string.Format("Can't find sprite {0} for clip {1}", frame._name, clip._name));
						}
					}
					frames.Add(frame);
				}
			}


			if (frames.Count == 0) {
				throw new System.Exception(string.Format("Clip {0} with no frames.", clip._name));
			}
			clip._frames = frames;
		}
	}

	GameObject CreateAnimationCollection( string target ) {
        GameObject go = new GameObject();
        tk2dSpriteAnimation anim = go.AddComponent<tk2dSpriteAnimation>();
        anim.clips = new tk2dSpriteAnimationClip[0];
        tk2dEditorUtility.SetGameObjectActive(go, false);

		Object p = PrefabUtility.CreateEmptyPrefab(target);
        PrefabUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(go);

        return AssetDatabase.LoadAssetAtPath(target, typeof(GameObject)) as GameObject;
	}

	public void BuildOrUpdate( string targetPath, tk2dDDLNS.AnimationCollection dataDefinition ) {
		targetPath = Regex.Replace(targetPath, "/+", "/"); // clean up duplicate 

		System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetPath));
		AssetDatabase.Refresh();

		// Create or load the prefab
		// This is considerably more complicated than it needs to be because it is a prefab.
		// It should be a scriptableobject but we can't change that because - backwards compatibility.
		GameObject targetGo = AssetDatabase.LoadAssetAtPath(targetPath, typeof(GameObject)) as GameObject;
		if (targetGo == null) {
			targetGo = CreateAnimationCollection(targetPath);
	
			// Make sure the index is up to date
			tk2dEditorUtility.GetOrCreateIndex().AddSpriteAnimation(targetGo.GetComponent<tk2dSpriteAnimation>());
			tk2dEditorUtility.CommitIndex();
		}
		tk2dSpriteAnimation collection = targetGo.GetComponent<tk2dSpriteAnimation>();
		if (collection == null) {
			throw new System.Exception("Unable to create animation at path: " + targetPath);
		}

		List<tk2dSpriteAnimationClip> targetClips = new List<tk2dSpriteAnimationClip>( collection.clips );
		List<bool> used = new List<bool>();
		for (int i = 0; i < targetClips.Count; ++i) {
			if (targetClips[i] == null) {
				targetClips[i] = new tk2dSpriteAnimationClip();
				targetClips[i].Clear();
			}
			used.Add(false);
		}

		// Do stuff
		foreach ( tk2dDDLNS.Clip clip in dataDefinition._clips ) {
			// match stuff up by name
			int foundId = -1;
			for (int i = 0; i < targetClips.Count; ++i) {
				if (targetClips[i].name == clip._name) {
					foundId = i;
					used[i] = true;
					break;
				}
			}
			// insert
			if (foundId == -1) {
				for (int i = 0; i < targetClips.Count; ++i) {
					if (targetClips[i].Empty) {
						foundId = i;
						used[i] = true;
						break;
					}
				}
			}
			if (foundId == -1) {
				tk2dSpriteAnimationClip newTarget = new tk2dSpriteAnimationClip();
				newTarget.frames = new tk2dSpriteAnimationFrame[0];
				foundId = targetClips.Count;
				targetClips.Add( newTarget );
				used.Add(true);
			}

			// Fill up clip
			FillClip(targetClips[foundId], clip);
		}

		// Delete unused while maintaining indices
		for (int i = 0; i < used.Count; ++i) {
			if (!used[i]) {
				targetClips[i].Clear();
			}
		}

		// Assign
		collection.clips = targetClips.ToArray();

		// Make sure its saved first
		EditorUtility.SetDirty( collection );
		AssetDatabase.SaveAssets();
	}

	tk2dSpriteCollectionData FindTargetSpriteCollection(string name) {
		if (spriteCollectionLUT.ContainsKey(name)) {
			return spriteCollectionLUT[name];
		}

		tk2dSpriteCollectionData data = null;

		tk2dSpriteCollectionIndex[] index = tk2dEditorUtility.GetOrCreateIndex().GetSpriteCollectionIndex();
		foreach (tk2dSpriteCollectionIndex indexItem in index) {
			if (indexItem.name.ToLower() == name) {
				data = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( indexItem.spriteCollectionDataGUID ), typeof(tk2dSpriteCollectionData) ) as tk2dSpriteCollectionData;
				break;
			}
		}

		if (data == null) {
			throw new System.Exception(string.Format("Unable to load sprite collection '{0}'", name));
		}
		spriteCollectionLUT[name] = data;

		return data;
	}

	int FindSpriteInTargetCollection(tk2dSpriteCollectionData data, string name) {
		data = data.inst;
		for (int i = 0; i < data.spriteDefinitions.Length; ++i) {
			if (data.spriteDefinitions[i] != null && data.spriteDefinitions[i].name.ToLower() == name) {
				return i;
			}
		}

		throw new System.Exception(string.Format("Unable to find sprite '{0}'' in sprite collection {1}", name, data.spriteCollectionName));
	}

	void FillClip(tk2dSpriteAnimationClip target, tk2dDDLNS.Clip source) {
		target.name = source._name;
		target.frames = new tk2dSpriteAnimationFrame[ source._frames.Count ];
		target.fps = source._fps;
		target.wrapMode = source._wrapMode;
		target.loopStart = source._loopStart;
		for (int i = 0; i < source._frames.Count; ++i) {
			tk2dDDLNS.Frame sourceFrame = source._frames[i];
			tk2dSpriteAnimationFrame targetFrame = new tk2dSpriteAnimationFrame();

			// Find the collection
			targetFrame.spriteCollection = FindTargetSpriteCollection( sourceFrame._collectionName );
			targetFrame.spriteId = FindSpriteInTargetCollection( targetFrame.spriteCollection, sourceFrame._name );

			if (sourceFrame._triggerDefinition == null) {
				targetFrame.triggerEvent = false;
				targetFrame.eventInfo = "";
				targetFrame.eventInt = 0;
				targetFrame.eventFloat = 0;
			}
			else {
				targetFrame.triggerEvent = true;
				targetFrame.eventInfo = sourceFrame._triggerDefinition._stringValue;
				targetFrame.eventInt = sourceFrame._triggerDefinition._intValue;
				targetFrame.eventFloat = sourceFrame._triggerDefinition._floatValue;
			}


			target.frames[i] = targetFrame;
		}
	}
}

class tk2dDataDefinitionSpriteCollectionBuilder {
	bool CheckPath(string path) {
			if (!System.IO.File.Exists(path)) {
			throw new System.Exception("Unable to find path: " + path);
		}
		return true;
	}

	string[] FindFilesAtPath(string path) {
		path = path.Replace("//", "/");

		int f = path.LastIndexOf('/');
		if (f == -1) {
			throw new System.Exception("Error matching wildcard: " + path);
		}
		string wildcard = path.Substring(f + 1).ToLower();
		Regex regex = new Regex("^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$");
		string dir = path.Substring(0, f);
		string[] allFiles = System.IO.Directory.GetFiles(dir);

		List<string> allMatchingFileNames = new List<string>();
		foreach (string _theFile in allFiles) {
			string theFile = _theFile.Replace("\\", "/");
			string fname = System.IO.Path.GetFileName(theFile);
			if (regex.IsMatch(fname.ToLower())) {
				allMatchingFileNames.Add(dir + "/" + fname);
			}
		}

		if (allMatchingFileNames.Count == 0) {
			throw new System.Exception("Wildcard did not match any files " + path);			
		}

		return allMatchingFileNames.ToArray();
	}

	// Takes the sprite collection, finds sprite names with wildcards
	public void FinalizeSpriteWildcards(string defBasePath, tk2dDDLNS.SpriteCollection collection) {
		List<tk2dDDLNS.Sprite> realSprites = new List<tk2dDDLNS.Sprite>();
		string basePath = "Assets/" + defBasePath + "/" + collection.InputPath;
		basePath = basePath.Replace("\\", "/").Replace("//", "/");
		foreach (tk2dDDLNS.Sprite s in collection._sprites) {
			if (s._path.Contains("?") || s._path.Contains("*")) {
				string[] paths = FindFilesAtPath(basePath + "/" + s._path);
				foreach (string p in paths) {
					tk2dDDLNS.Sprite sprite = s._Copy;
					sprite._path = p;
					realSprites.Add(sprite);
				}
			}
			else {
				if (CheckPath(basePath + "/" + s._path)) {
					s._path = basePath + "/" + s._path;
					realSprites.Add(s);
				}
			}
		}
		collection._sprites = realSprites;

		// Convert to full path
		foreach (tk2dDDLNS.Sprite s in collection._sprites) {
			if (CheckPath(s._path)) {
				s._spriteName = System.IO.Path.GetFileNameWithoutExtension(s._path);
				int idx = s._spriteName.LastIndexOf('@'); // strip out @2x, etc
				if (idx != -1) {
					s._spriteName = s._spriteName.Substring(0, idx);
				}
			}
		}
	}

	GameObject CreateSpriteCollection( string target ) {
        GameObject go = new GameObject();
        tk2dSpriteCollection spriteCollection = go.AddComponent<tk2dSpriteCollection>();
        spriteCollection.version = tk2dSpriteCollection.CURRENT_VERSION;
        if (tk2dCamera.Editor__Inst != null) {
        	spriteCollection.sizeDef.CopyFrom( tk2dSpriteCollectionSize.ForTk2dCamera( tk2dCamera.Editor__Inst ) );
        }
        tk2dEditorUtility.SetGameObjectActive(go, false);

		Object p = PrefabUtility.CreateEmptyPrefab(target);
        PrefabUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(go);

        return AssetDatabase.LoadAssetAtPath(target, typeof(GameObject)) as GameObject;
	}

	public void BuildOrUpdate( string targetPath, tk2dDDLNS.SpriteCollection dataDefinition ) {
		targetPath = Regex.Replace(targetPath, "/+", "/");
		dataDefinition._targetPath = targetPath;

		System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetPath));
		AssetDatabase.Refresh();

		// Create or load the prefab
		// This is considerably more complicated than it needs to be because it is a prefab.
		// It should be a scriptableobject but we can't change that because - backwards compatibility.
		GameObject targetGo = AssetDatabase.LoadAssetAtPath(targetPath, typeof(GameObject)) as GameObject;
		if (targetGo == null) {
			targetGo = CreateSpriteCollection(targetPath);
		}
		tk2dSpriteCollection collection = targetGo.GetComponent<tk2dSpriteCollection>();
		if (collection == null) {
			throw new System.Exception("Unable to create sprite collection at path: " + targetPath);
		}

		// Create sprites
		List<tk2dSpriteCollectionDefinition> defs = new List<tk2dSpriteCollectionDefinition>(collection.textureParams);
		List<bool> used = new List<bool>();

		// Use the first found definition as a template
		for (int i = 0; i < defs.Count; ++i) {
			if (defs[i] == null) {
				defs[i] = new tk2dSpriteCollectionDefinition();
			}			
			used.Add( false );
		}

		foreach (tk2dDDLNS.Sprite sprite in dataDefinition._sprites) {
			Texture2D tex = AssetDatabase.LoadAssetAtPath(sprite._path, typeof(Texture2D)) as Texture2D;
			if (tex == null) {
				throw new System.Exception("Unable to load texture " + sprite._path);
			}

			int foundId = -1;

			// Only add textures that aren't already in the collection
			for (int i = 0; i < defs.Count; ++i) {
				if (defs[i].name == sprite._spriteName && defs[i].texture != null) {
					foundId = i;
					used[i] = true;
					break;
				}
			}

			// Attempt insert into unused slot
			if (foundId == -1) {
				for (int i = 0; i < defs.Count; ++i) {
					if (defs[i].texture == null) {
						foundId = i;
						used[i] = true;
					}
				}
			}

			if (foundId == -1) {
				foundId = defs.Count;
				defs.Add(new tk2dSpriteCollectionDefinition());
				used.Add(true);
			}

			// Fill up sprite
			FillSprite( defs[foundId], sprite, tex );
		}
		
		// Delete unused while maintaining indices
		for (int i = 0; i < used.Count; ++i) {
			if (!used[i]) {
				defs[i].Clear();
			}
		}
		collection.textureParams = defs.ToArray();

		FillCollectionSettings(collection, dataDefinition);

		// Call all relevant callbacks
		if ( dataDefinition.OnPreBuildCollection != null ) {
			dataDefinition.OnPreBuildCollection( collection );
		}
		if ( dataDefinition.OnPreBuildSprite != null ) {
			foreach (tk2dSpriteCollectionDefinition def in collection.textureParams) {
				dataDefinition.OnPreBuildSprite( def );
			}
		}

		// Make sure its saved first
		EditorUtility.SetDirty( collection );
		AssetDatabase.SaveAssets();

		// And now build it
		tk2dSpriteCollectionBuilder.Rebuild( collection );
	}

	void FillSprite( tk2dSpriteCollectionDefinition target, tk2dDDLNS.Sprite source, Texture2D texture ) {
		target.texture = texture;
		target.name = source._spriteName;
		target.anchor = source._anchor;
		target.anchorX = source._anchorX;
		target.anchorY = source._anchorY;
	}

	void FillPlatformSettings(tk2dSpriteCollection target, tk2dDDLNS.SpriteCollection source) {
		if (source._platforms.Count == 0) {
			target.platforms.Clear();
		}
		else {
			List<tk2dSpriteCollectionPlatform> currentPlatforms = new List<tk2dSpriteCollectionPlatform>();
			int index = -1;
			foreach (string platform in source._platforms) {
				index = target.platforms.FindIndex( a => a.name == platform );
				if (index == -1) {
					tk2dSpriteCollectionPlatform p = new tk2dSpriteCollectionPlatform();
					p.name = platform;
					currentPlatforms.Add(p);
				}
				else {
					currentPlatforms.Add(target.platforms[index]);
				}
			}
			index = currentPlatforms.FindIndex(a => a.name == source._platforms[0]);
			if (index == -1) {
				throw new System.Exception("Platform not found " + source._platforms[0]);
			}
			else if (index != 0) {
				tk2dSpriteCollectionPlatform p = currentPlatforms[index];
				currentPlatforms.RemoveAt(index);
				currentPlatforms.Insert(0, p);
			}
			target.platforms = currentPlatforms;
		}
	}

	void FillCollectionSettings(tk2dSpriteCollection target, tk2dDDLNS.SpriteCollection source) {
		target.sizeDef = tk2dSpriteCollectionSize.PixelsPerMeter(source._pixelsPerMeter);

		FillPlatformSettings(target, source);
	}
}

public class tk2dDataDefinitionBuilder {

	public static tk2dDataDefinition[] FindDefinitionsInProject() {
		List<tk2dDataDefinition> allDataDefinitions = new List<tk2dDataDefinition>();
		foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
		{
			try
			{
				System.Type[] types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.BaseType == typeof(tk2dDataDefinition))
					{
						tk2dDataDefinition inst = (tk2dDataDefinition)System.Activator.CreateInstance(type);
						if (inst != null)
							allDataDefinitions.Add(inst);
					}
				}
			}
			catch { }
		}
		return allDataDefinitions.ToArray();
	}

	public static string Build(tk2dDataDefinition[] defs) {
		logData = ""; // clear before we start
		foreach (tk2dDataDefinition d in defs) {
			d.Define();
			Build(d);
		}
		return logData;
	}

	public static string logData = "";

	static void Build(tk2dDataDefinition data) {
		logData += string.Format("Building data definition {0}\n", data._Name);
		
		try {
			foreach (var v in data._allCollections) {
				
				{
					tk2dDataDefinitionSpriteCollectionBuilder spriteBuilder = new tk2dDataDefinitionSpriteCollectionBuilder();
					spriteBuilder.FinalizeSpriteWildcards(data._settings.InputPath, v);
					spriteBuilder.BuildOrUpdate( "Assets/" + data._settings.OutputPath + "/" + v.OutputPath + "/" + v._name + ".prefab", v );
					spriteBuilder = null;
				}

				System.GC.Collect();
				Resources.UnloadUnusedAssets();
				logData += string.Format("  Sprite Collection '{0}' with {1} sprites\n", v._name, v._sprites.Count); 
			}

			tk2dDataDefinitionAnimationBuilder animationBuilder = new tk2dDataDefinitionAnimationBuilder(data._allCollections);
			foreach (var v in data._allAnimations) {
				animationBuilder.FinalizeAnimationWildcards( v );
				animationBuilder.BuildOrUpdate( "Assets/" + data._settings.OutputPath + "/" + v.OutputPath + "/" + v._name + ".prefab", v);
				System.GC.Collect();
				Resources.UnloadUnusedAssets();
				logData += string.Format("  Animation Collection '{0}' with {1} clips\n", v._name, v._clips.Count); 
			}
		}
		catch (System.Exception e) {
			throw e;
		}
	}
}


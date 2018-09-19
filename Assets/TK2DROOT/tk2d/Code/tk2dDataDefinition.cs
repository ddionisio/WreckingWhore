using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace tk2dDDLNS {
	public class Settings {
		public string InputPath = "";  // Assets
		public string OutputPath = ""; // Assets

		public float PixelsPerMeter = 100.0f;
	}

	public class Sprite {
		public Sprite(SpriteCollection spriteCollection, string path) {
			this._path = path;
			this._spriteCollection = spriteCollection;
		}

		public Sprite _Copy { get { return this.MemberwiseClone() as Sprite; } }
		public string _path; // May have wildcards
		public string _spriteName = "";
		public tk2dSpriteCollectionDefinition.Anchor _anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
		public int _anchorX, _anchorY;
		SpriteCollection _spriteCollection;

		// Functions
		public Sprite Add(string path) {
			return _spriteCollection._AddSprite(path);
		}

		public Sprite Anchor(string val) {
			_anchor = Helper.ParseEnum<tk2dSpriteCollectionDefinition.Anchor>(val);
			if (_anchor == tk2dSpriteCollectionDefinition.Anchor.Custom) {
				throw new System.Exception("Custom anchor requires coordinates");
			}
			return this;
		}

		public Sprite Anchor(string val, int x, int y) {
			_anchor = Helper.ParseEnum<tk2dSpriteCollectionDefinition.Anchor>(val);
			_anchorX = x;
			_anchorY = y;
			return this;
		}
	}

	public class SpriteCollection {
		public SpriteCollection(tk2dDataDefinition dataDefinition, string name) {
			this._name = name;
			this._dataDefinition = dataDefinition;
			_AddSprite(null);
		}

		public string _name = "";
		public string _textureFormat = "TrueColor";
		public string _targetPath = ""; // assigned during build
		public List<string> _platforms = new List<string>();
		public List<Sprite> _sprites = new List<Sprite>();
		public Sprite _sprite;
		tk2dDataDefinition _dataDefinition;
		public float _pixelsPerMeter = 100;

		public Sprite _AddSprite(string spritePath) {
			_sprite = new Sprite(this, spritePath);
			if (spritePath != null) {
				_sprites.Add(_sprite);
			}
			return _sprite;
		}

		// Variables
		public string InputPath = "";  // Settings.input_path
		public string OutputPath = ""; // Settings.output_path

		// Events
		public System.Action<tk2dSpriteCollection> OnPreBuildCollection = null;
		public System.Action<tk2dSpriteCollectionDefinition> OnPreBuildSprite = null;

		// Functions
		public SpriteCollection Add(string name) {
			return Add(name, -1);
		}

		public SpriteCollection Add(string name, float pixelsPerMeter) {
			SpriteCollection sc = _dataDefinition._AddSpriteCollection(name);
			sc._pixelsPerMeter = (pixelsPerMeter == -1) ? _dataDefinition._settings.PixelsPerMeter : pixelsPerMeter;
			return sc;
		}

		public SpriteCollection Platforms(params string[] platforms) {
			_platforms.Clear();
			_platforms.AddRange(platforms);
			return this;
		}
	}

	public static class Helper {

		public static Dictionary<System.Type, string> friendlyNames = new Dictionary<System.Type, string>();
		static Helper() {
			friendlyNames.Add(typeof(tk2dSpriteAnimationClip.WrapMode), "WrapMode");
		}

		public static T ParseEnum<T>(string str) {
			try {
				return (T)System.Enum.Parse(typeof(T), str, true);
			}
			catch (System.Exception) {
				string typeName = friendlyNames.ContainsKey(typeof(T)) ? friendlyNames[typeof(T)] : (typeof(T)).ToString();
				throw new System.Exception(string.Format("Error parsing {0}: '{1}'", typeName, str));
			}
		}
	}

	public class Clip {
		public Clip(tk2dDataDefinition dataDefinition, AnimationCollection animationCollection, string name) {
			this._name = name;
			this._dataDefinition = dataDefinition;
			this._animationCollection = animationCollection;
			_AddFrame(null);
		}

		public Frame _AddFrame(string name) {
			_frame = new Frame(_dataDefinition, this, name);
			if (name != null) {
				_frames.Add(_frame);
			}
			return _frame;
		}

		public string _name = "";
		public tk2dSpriteAnimationClip.WrapMode _wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
		public float _fps = -1;
		public int _loopStart = 0;
		public List<Frame> _frames = new List<Frame>();
		public Frame _frame = null;

		tk2dDataDefinition _dataDefinition;
		AnimationCollection _animationCollection;

		// Functions
		public Clip Add(string name, string wrapMode) {
			return Add(name, wrapMode, -1);
		}

		public Clip Add(string name, string wrapMode, float fps) {
			Clip c = _animationCollection._AddClip(name);
			c._wrapMode = Helper.ParseEnum<tk2dSpriteAnimationClip.WrapMode>(wrapMode);
			if (fps != -1) {
				c._fps = fps;
			}
			return c;
		}

		public void LoopStart(int start) {
			if (_wrapMode != tk2dSpriteAnimationClip.WrapMode.LoopSection) {
				throw new System.Exception("Clip: Only allowed to set LoopStart when wrapmode is LoopSection");
			}
			_loopStart = start;
		}
	}

	public class Frame {
		public Frame(tk2dDataDefinition dataDefinition, Clip clip, string name) {
			int v = name != null ? name.IndexOf(":") : -1;
			if (v != -1) {
				this._collectionName = name.Substring(0, v);
				this._name = name.Substring(v + 1, name.Length - v - 1);
			}
			else {
				this._collectionName = null;
				this._name = name;
			}
			this._dataDefinition = dataDefinition;
			this._clip = clip;
		}

		public Frame _Copy { get { return this.MemberwiseClone() as Frame; } }
		public string _collectionName = null;
		public string _name = "";
		public bool _reverse = false;
		public int _rangeMin = -1, _rangeMax = -1;
		public TriggerDefinition _triggerDefinition = null;

		tk2dDataDefinition _dataDefinition;
		Clip _clip;

		// Functions
		public Frame Reverse() {
			_reverse = !_reverse;
			return this;
		}

		public Frame Range(int min, int max) {
			if (max < min) {
				_reverse = true;
				_rangeMin = max;
				_rangeMax = min;
			}
			else {
				_reverse = false;
				_rangeMin = min;
				_rangeMax = max;
			}
			return this;
		}

		public Frame Trigger(string name) {
			_triggerDefinition = _dataDefinition._FindTrigger(name);
			return this;
		}

		public Frame Add(string name) {
			Frame f = _clip._AddFrame(name);
			return f;
		}
	}

	public class TriggerDefinition {
		public string _stringValue = "";
		public float _floatValue = 0;
		public int _intValue = 0;
	}

	public class AnimationCollection {
		public AnimationCollection(tk2dDataDefinition dataDefinition, string name) {
			this._name = name;
			this._dataDefinition = dataDefinition;
			_AddClip(null);
		}

		tk2dDataDefinition _dataDefinition = null;
		public string _name = "";

		public Clip _clip = null;
		public List<Clip> _clips = new List<Clip>();

		public Clip _AddClip(string clipName) {
			_clip = new Clip(_dataDefinition, this, clipName);
			_clip._fps = DefaultFps;
			if (clipName != null) {
				_clips.Add(_clip);
			}
			return _clip;
		}

		// Variables
		public string OutputPath = ""; // Settings.output_path
		public float DefaultFps = 30.0f;

		// Functions
		public AnimationCollection Add(string name) {
			return _dataDefinition._AddAnimationCollection(name);
		}
	}
}

public abstract class tk2dDataDefinition {

	public string _Name {
		get {
			return this.GetType().ToString();
		}
	}

	public List<tk2dDDLNS.SpriteCollection> _allCollections = new List<tk2dDDLNS.SpriteCollection>();
	public List<tk2dDDLNS.AnimationCollection> _allAnimations = new List<tk2dDDLNS.AnimationCollection>();
	public Dictionary<string, tk2dDDLNS.TriggerDefinition> _allTriggers = new Dictionary<string, tk2dDDLNS.TriggerDefinition>();

	private tk2dDDLNS.SpriteCollection _collection = null;
	private tk2dDDLNS.AnimationCollection _animation = null;
	public tk2dDDLNS.Settings _settings = new tk2dDDLNS.Settings();

	public tk2dDDLNS.TriggerDefinition _FindTrigger(string name) {
		name = name.ToLower();
		if (!_allTriggers.ContainsKey(name)) {
			throw new System.Exception(string.Format("Unable to find trigger named {0}. Did you forget to DefineTrigger?", name));
		}
		return _allTriggers[name];
	}
	public tk2dDDLNS.SpriteCollection _AddSpriteCollection(string name) {
		_collection = new tk2dDDLNS.SpriteCollection(this, name);
		_allCollections.Add(_collection);
		return _collection;
	}

	public tk2dDDLNS.AnimationCollection _AddAnimationCollection(string name) {
		_animation = new tk2dDDLNS.AnimationCollection(this, name);
		_allAnimations.Add(_animation);
		return _animation;
	}


	// Shortcuts
	protected tk2dDDLNS.Sprite Sprite { get { return _collection._sprite; } }
	protected tk2dDDLNS.Clip Clip { get { return _animation._clip; } }
	protected tk2dDDLNS.Frame Frame { get { return _animation._clip._frame; } }
	
	protected void DefineTrigger(string name, string stringValue, int intValue, float floatValue) {
		name = name.ToLower();
		if (_allTriggers.ContainsKey(name)) {
			throw new System.Exception(string.Format("Duplicate trigger definition: {0}. Trigger names are not case sensitive.", name));
		}
		tk2dDDLNS.TriggerDefinition t = new tk2dDDLNS.TriggerDefinition();
		t._stringValue = stringValue;
		t._intValue = intValue;
		t._floatValue = floatValue;
		_allTriggers[name] = t;
	}

	protected tk2dDDLNS.Settings Settings { 
		get {
			return _settings;
		} 
	}

	protected tk2dDDLNS.SpriteCollection SpriteCollection {
		get {
			if (_collection == null) {
				_collection = new tk2dDDLNS.SpriteCollection(this, "-");
			}
			return _collection;
		}
	}

	protected tk2dDDLNS.AnimationCollection AnimationCollection {
		get {
			if (_animation == null) {
				_animation = new tk2dDDLNS.AnimationCollection(this, "-");
			}
			return _animation;
		}
	}

	// Abstract
	public abstract void Define(); 
}



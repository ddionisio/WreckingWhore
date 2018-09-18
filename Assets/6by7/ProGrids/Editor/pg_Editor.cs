using UnityEngine;
using UnityEditor;
using SixBySeven;
using System.Collections;
using System.Reflection;

[System.Serializable]
[InitializeOnLoad]
public class pg_Editor : EditorWindow
{

#region MEMBERS

	public static pg_Editor pg;

	private bool snapEnabled = true;
	private SnapUnit snapUnit = SnapUnit.Meter;
	private float snapValue = 1f;						// the actual snap value, taking into account unit size
	private float t_snapValue = 1f;					// what the user sees

	private bool drawGrid = true;
	private bool drawAngles = false;
	private float angleValue = 45f;
#endregion

#region CONSTANT
	const float METER = 1f;
	const float CENTIMETER = .001f;
	const float INCH = 0.0253999862840074f;
	const float YARD = 1.09361f;
	const float PARSEC = 5f;

	const int MAX_LINES = 150;							// the maximum amount of lines to display on screen in either direction

	const int BUTTON_SIZE = 36;
	private Texture2D gui_SnapToGrid;
	private Texture2D gui_SnapOn;
	private Texture2D gui_SnapOff;
	private Texture2D gui_VisOn;
	private Texture2D gui_VisOff;
	private Texture2D gui_AnglesOn;
	private Texture2D gui_AnglesOff;
#endregion

#region PREFERENCES
	/** Defaults **/
	public Color GRID_COLOR_X = new Color(.9f, .46f, .46f, .447f);
	public Color GRID_COLOR_Y = new Color(.46f, .9f, .46f, .447f);
	public Color GRID_COLOR_Z = new Color(.46f, .46f, .9f, .447f);

	/** Settings **/
	public Color gridColorX, gridColorY, gridColorZ;

	public void LoadPreferences()
	{
		gridColorX = (EditorPrefs.HasKey("gridColorX")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorX")) : GRID_COLOR_X;
		gridColorY = (EditorPrefs.HasKey("gridColorY")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorY")) : GRID_COLOR_Y;
		gridColorZ = (EditorPrefs.HasKey("gridColorZ")) ? pg_Util.ColorWithString(EditorPrefs.GetString("gridColorZ")) : GRID_COLOR_Z;
	}

	private GUISkin sixBySevenSkin;
#endregion

#region ENUM

	public enum Axes {
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ
	}

	public enum SnapUnit {
		Meter,
		Centimeter,
		Inch,
		Yard,
		Parsec
	}

	public float SnapUnitValue(SnapUnit su) {
		switch(su)
		{
			case SnapUnit.Meter:
				return METER;
			case SnapUnit.Centimeter:
				return CENTIMETER;
			case SnapUnit.Inch:
				return INCH;
			case SnapUnit.Yard:
				return YARD;
			case SnapUnit.Parsec:
				return PARSEC;
			default:
				return METER;
		}
	}
#endregion

#region INITIALIZATION

	[MenuItem("Window/ProGrids/ProGrids Window")]
	public static void InitProGrids()
	{
		EditorWindow.GetWindow(typeof(pg_Editor), false, "PG", true).autoRepaintOnSceneChange = true;
		SceneView.RepaintAll();
	}

	// hax
	public void OnInspectorGUI()
	{
		if(EditorWindow.focusedWindow != this)
		{
			Repaint();
		}
	}

	public void OnEnable()
	{
		pg = this;

		HookSceneView();
		LoadGUIResources();
		LoadPreferences();
		autoRepaintOnSceneChange = true;
		SetSharedSnapValues(snapEnabled, snapValue);

		this.minSize = new Vector2(BUTTON_SIZE+4, BUTTON_SIZE*6);
		this.maxSize = new Vector2(BUTTON_SIZE+4, BUTTON_SIZE*6);
	}

	public void OnFocus()
	{
		SetSharedSnapValues(snapEnabled, snapValue);
		SceneView.RepaintAll();
	}

	public void OnDisable()
	{
		SceneView.RepaintAll();
		SetSharedSnapValues(false, snapValue);
	}

	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	public void HookSceneView()
	{
		if(SceneView.onSceneGUIDelegate != this.OnSceneGUI)
		{
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}
	}
#endregion 

#region INTERFACE

	RectOffset pad = new RectOffset(0,0,0,0);
	RectOffset oldPad = new RectOffset(2,2,2,2);
	GUIStyle toggleStyle = GUIStyle.none;
	const int TOGGLE_WIDTH = 20;

	public void OnGUI()
	{
		GUI.skin.button.padding = pad;

		if(pgButton(gui_SnapToGrid))
			SnapToGrid(Selection.transforms);

		if(pgButton((snapEnabled) ? gui_SnapOn : gui_SnapOff))
			SetSnapEnabled(!snapEnabled);

		// gui_VisEnabled = ;
		if(pgButton((drawGrid) ? gui_VisOn : gui_VisOff))
			SetGridEnabled(!drawGrid);
		
		// if(pgButton((drawAngles) ? gui_AnglesOn : gui_AnglesOff, GUIStyle.none))		
		if(GUILayout.Button( (drawAngles) ? gui_AnglesOn : gui_AnglesOff, toggleStyle))
			SetDrawAngles(!drawAngles);

		if(drawAngles)
		{
			EditorGUI.BeginChangeCheck();
			angleValue = EditorGUILayout.FloatField("", angleValue, 
				GUILayout.MinWidth(BUTTON_SIZE),
				GUILayout.MaxWidth(BUTTON_SIZE));
			if(EditorGUI.EndChangeCheck()) {
				SceneView.RepaintAll();
			}
		}

		EditorGUI.BeginChangeCheck();
			snapUnit = (SnapUnit)EditorGUILayout.EnumPopup("", snapUnit,
				GUILayout.MinWidth(BUTTON_SIZE),
				GUILayout.MaxWidth(BUTTON_SIZE));

		t_snapValue = EditorGUILayout.FloatField("", t_snapValue,
			GUILayout.MinWidth(BUTTON_SIZE),
			GUILayout.MaxWidth(BUTTON_SIZE));
		if(EditorGUI.EndChangeCheck()) {
			SetSnapValue(snapUnit, t_snapValue);
		}

		GUI.skin.button.padding = oldPad;
	}

	public void LoadGUIResources()
	{
		toggleStyle.margin = new RectOffset(5,5,5,5);

		gui_SnapToGrid  = (Texture2D)Resources.Load("GUI/ProGridsGUI_SnapToGrid");
		gui_SnapOn 		= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_OnLight");
		gui_SnapOff		= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_OffLight");
		gui_VisOn		= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_VisOn");
		gui_VisOff		= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_VisOff");
		gui_AnglesOn	= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_AnglesOn");
		gui_AnglesOff	= (Texture2D)Resources.Load("GUI/ProGridsToggles/ProGridsGUI_AnglesOff");
	}

	public bool pgButton(Texture2D img)
	{
		if(GUILayout.Button(img))
			// , 
			// GUILayout.MinWidth(BUTTON_SIZE), GUILayout.MaxWidth(BUTTON_SIZE),
			// GUILayout.MinHeight(BUTTON_SIZE), GUILayout.MaxHeight(BUTTON_SIZE)))
			return true;
		return false;
	}

	public bool pgButton(Texture2D img, GUIStyle style)
	{
		if(GUILayout.Button(img, style,
			GUILayout.MinWidth(BUTTON_SIZE), GUILayout.MaxWidth(BUTTON_SIZE),
			GUILayout.MinHeight(BUTTON_SIZE), GUILayout.MaxHeight(BUTTON_SIZE)))
			return true;
		return false;
	}
#endregion

#region ONSCENEGUI

	private Transform lastTransform;
	
	public Vector3 lastPosition = Vector3.zero;

	public void OnSceneGUI(SceneView scnview)
	{
		Event e = Event.current;

		Camera cam = Camera.current;
		
		if(cam.orthographic && drawGrid)
			DrawGrid(cam);

		if(e.type == EventType.ValidateCommand)
		{
			OnValidateCommand(Event.current.commandName);
			return;
		}

		// Always keep track of the selection
		if(!Selection.transforms.Contains(lastTransform))
		{
			if(Selection.activeTransform)
			{
				lastTransform = Selection.activeTransform;
				lastPosition = Selection.activeTransform.position;
			}
		}

		if(!snapEnabled || GUIUtility.hotControl < 1)
			return;
		
		/**
		 *	Snapping
		 */
		if(Selection.activeTransform)
		{		
			if(lastTransform.position != lastPosition)
			{
				Transform selected = lastTransform;

				Vector3 old = selected.position;
				Vector3 mask = old - lastPosition;

				selected.position = SnapValue(old, mask);

				Vector3 offset = selected.position - old;

				OffsetTransforms(Selection.transforms, selected, offset);

				lastPosition = selected.position;
			}
		}
	}
#endregion

#region GRAPHICS
 
	// [DrawGizmo(GizmoType.NotSelected)]
 //    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
	// {
	// 	if(pg != null)
	//  		pg.HookSceneView();
	// }

	public void DrawGrid(Camera cam)
	{
		Axes camAxis = AxesWithVector(Camera.current.transform.TransformDirection(Vector3.forward).normalized);

		if(drawGrid) {
			switch(camAxis)
			{
				case Axes.X:
				case Axes.NegX:
					DrawGridGraphics(cam, camAxis, gridColorX);
					break;

				case Axes.Y:
				case Axes.NegY:
					DrawGridGraphics(cam, camAxis, gridColorY);
					break;

				case Axes.Z:
				case Axes.NegZ:
					DrawGridGraphics(cam, camAxis, gridColorZ);
					break;
			}
		}
	}

	Color previousColor;
	private void DrawGridGraphics(Camera cam, Axes camAxis, Color col)
	{
		previousColor = Handles.color;
		Handles.color = col;
	
		// !-- TODO: Update this stuff only when necessary.  Currently it runs evvverrrryyy frame
		Vector3 bottomLeft 	= SnapToFloor(cam.ScreenToWorldPoint(Vector2.zero));
		Vector3 bottomRight = SnapToFloor(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, 0f)));
		Vector3 topLeft 	= SnapToFloor(cam.ScreenToWorldPoint(new Vector2(0f, cam.pixelHeight)));
		Vector3 topRight 	= SnapToFloor(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight)));

		Vector3 axis = VectorWithAxes(camAxis);

		float width = Vector3.Distance(bottomLeft, bottomRight);
		float height = Vector3.Distance(bottomRight, topRight);

		// Shift lines to 1m forward of the camera
		bottomLeft 	+= axis*2;
		topRight 	+= axis*2;
		bottomRight += axis*2;
		topLeft 	+= axis*2;

		/** 
		 *	Draw Vertical Lines 
		 *	Add two because we want the grid to cover the entire screen.
		 */
		Vector3 start = bottomLeft - cam.transform.up * (height+snapValue*2);
		Vector3 end = bottomLeft + cam.transform.up * (height+snapValue*2);
		
		float _snapVal = snapValue;

		int segs = (int)Mathf.Ceil(width / _snapVal) + 2;

		float n = 2;
		while(segs > MAX_LINES) {
			_snapVal = _snapVal*n;
			segs = (int)Mathf.Ceil(width / _snapVal );
			n++;
		}

		for(int i = -1; i < segs; i++)
		{
			Handles.DrawLine(
				start + (i * (cam.transform.right * _snapVal) ),
				end + (i * (cam.transform.right * _snapVal) ) );
		}

		/** 
		 * Draw Horizontal Lines
		 */
		start = topLeft - cam.transform.right * (width+snapValue*2);
		end = topLeft + cam.transform.right * (width+snapValue*2);

		segs = (int)Mathf.Ceil(height / _snapVal) + 2;

		n = 1;
		while(segs > MAX_LINES) {
			_snapVal = _snapVal*n;
			n++;
			segs = (int)Mathf.Ceil(height / _snapVal );
		}

		for(int i = -1; i < segs; i++)
		{
			Handles.DrawLine(
				start + (i * (-cam.transform.up * _snapVal)) ,
				end + (i * (-cam.transform.up * _snapVal)) );
		}

		if(drawAngles)
		{
			Vector3 cen = SnapValue(((topRight + bottomLeft) / 2f));

			float half = (width > height) ? width : height;

			float opposite = Mathf.Tan( Mathf.Deg2Rad*angleValue ) * half;

			Vector3 up = cam.transform.up * opposite;
			Vector3 right = cam.transform.right * half;

			Vector3 bottomLeftAngle 	= cen - (up+right);
			Vector3 topRightAngle 		= cen + (up+right);

			Vector3 bottomRightAngle	= cen + (right-up);
			Vector3 topLeftAngle 		= cen + (up-right);

			// y = 1x+1
			Handles.DrawLine(bottomLeftAngle, topRightAngle);

			// y = -1x-1
			Handles.DrawLine(topLeftAngle, bottomRightAngle);	
		}

		Handles.color = previousColor;
	}
#endregion

#region ENUM UTILITY
	
	public Axes AxesWithVector(Vector3 val)
	{
		Vector3 v = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), Mathf.Abs(val.z));

		if(v.x > v.y && v.x > v.z) {
			if(val.x > 0)
				return Axes.X;
			else
				return Axes.NegX;
		}
		else
		if(v.y > v.x && v.y > v.z) {
			if(val.y > 0)
				return Axes.Y;
			else
				return Axes.NegY;			
		}
		else {
			if(val.z > 0)
				return Axes.Z;
			else
				return Axes.NegZ;
		}
	}

	public Vector3 VectorWithAxes(Axes axis)
	{
		switch(axis)
		{
			case Axes.X:
				return Vector3.right;
			case Axes.Y:
				return Vector3.up;
			case Axes.Z:
				return Vector3.forward;
			case Axes.NegX:
				return -Vector3.right;
			case Axes.NegY:
				return -Vector3.up;
			case Axes.NegZ:
				return -Vector3.forward;

			default:
				return Vector3.forward;
		}
	}
#endregion

#region EVENT

	bool oldVal;
	public void OnValidateCommand(string command)
	{
		switch(command)
		{
			case "UndoRedoPerformed":
				
				if(Selection.activeTransform)
				{
					lastTransform = Selection.activeTransform;
					lastPosition = Selection.activeTransform.position;
				}

				Event.current.Use();

				break;
		}
	}

	public void OnExecuteCommand()
	{
		Debug.Log("ExecuteCommand");
	}
#endregion

#region SNAP

	public void SnapToGrid(Transform[] transforms)
	{
		Undo.RegisterUndo(transforms as Object[], "Snap to Grid");
		foreach(Transform t in transforms)
		{
			t.position = SnapValue(t.position);
		}
	}

	public Vector3 SnapValue(Vector3 val)
	{
		float _x = val.x, _y = val.y, _z = val.z;
		return new Vector3(
			Snap(_x),
			Snap(_y),
			Snap(_z)
			);
	}

	public Vector3 SnapValue(Vector3 val, Vector3 mask)
	{
		float _x = val.x, _y = val.y, _z = val.z;
		return new Vector3(
			(Mathf.Approximately(mask.x, 0f) ? _x : Snap(_x) ),
			(Mathf.Approximately(mask.y, 0f) ? _y : Snap(_y) ),
			(Mathf.Approximately(mask.z, 0f) ? _z : Snap(_z) )
			);
	}

	public Vector3 SnapToCeil(Vector3 val, Vector3 mask)
	{
		float _x = val.x, _y = val.y, _z = val.z;
		return new Vector3(
			(Mathf.Approximately(mask.x, 0f) ? _x : SnapToCeil(_x) ),
			(Mathf.Approximately(mask.y, 0f) ? _y : SnapToCeil(_y) ),
			(Mathf.Approximately(mask.z, 0f) ? _z : SnapToCeil(_z) )
			);
	}

	public Vector3 SnapToFloor(Vector3 val)
	{
		float _x = val.x, _y = val.y, _z = val.z;
		return new Vector3(
			SnapToFloor(_x),
			SnapToFloor(_y),
			SnapToFloor(_z)
			);
	}

	public Vector3 SnapToFloor(Vector3 val, Vector3 mask)
	{
		float _x = val.x, _y = val.y, _z = val.z;
		return new Vector3(
			(Mathf.Approximately(mask.x, 0f) ? _x : SnapToFloor(_x) ),
			(Mathf.Approximately(mask.y, 0f) ? _y : SnapToFloor(_y) ),
			(Mathf.Approximately(mask.z, 0f) ? _z : SnapToFloor(_z) )
			);
	}

	public float Snap(float val)
	{
		return snapValue * Mathf.Round(val / snapValue);
	}

	public float SnapToFloor(float val)
	{
		return snapValue * Mathf.Floor(val / snapValue);
	}

	public float SnapToCeil(float val)
	{
		return snapValue * Mathf.Ceil(val / snapValue);
	}
#endregion

#region MOVING TRANSFORMS

	public void OffsetTransforms(Transform[] trsfrms, Transform ignore, Vector3 offset)
	{
		foreach(Transform t in trsfrms)
		{
			if(t != ignore)
				t.position += offset;
		}
	}
#endregion

#region SETTINGS

	/**
	 *	ALL SETTERS ARE RESPONSIBLE FOR UPDATING PROBUILDER
	 */
	public void SetSnapEnabled(bool enable)
	{
		if(Selection.activeTransform)
		{
			lastTransform = Selection.activeTransform;
			lastPosition = Selection.activeTransform.position;
		}

		snapEnabled = enable;
		SceneView.RepaintAll();
		SetSharedSnapValues(snapEnabled, snapValue);
	}

	public void SetSnapValue(SnapUnit su, float val)
	{
		snapValue = SnapUnitValue(su) * val;
		SceneView.RepaintAll();
		SetSharedSnapValues(snapEnabled, snapValue);
	}

	public void SetGridEnabled(bool enable)
	{
		drawGrid = enable;
		SceneView.RepaintAll();
	}

	public void SetDrawAngles(bool enable)
	{
		drawAngles = enable;
		SceneView.RepaintAll();
	}
#endregion

#region GLOBAL SETTING
	
	public void SetSharedSnapValues(bool enable, float snapVal)
	{
		// SixBySeven.Shared.SetSnap(enable, snapVal);
		SixBySeven.Shared.snapEnabled = enable;
		SixBySeven.Shared.snapValue = snapVal;

		// foreach(Transform t in Selection.transforms)
		// {
		// 	if(t.GetComponent("pb_Object")) // Magic strings are sometimes okay!
		// 	{
		// 		System.Type tp = t.GetComponent("pb_Object").GetType();
		// 		object obj = t.GetComponent("pb_Object");
		// 		// -- for whatever reason this doesn't work... [snapEnabled, snapValue];
		// 		object[] param = new object[2];
		// 		param[0] = enable;
		// 		param[1] = snapVal;
		// 		MethodInfo method = tp.GetMethod("OnProGridsChange");
		// 		method.Invoke( obj, param );
		// 	}
		// }
	}
#endregion
}
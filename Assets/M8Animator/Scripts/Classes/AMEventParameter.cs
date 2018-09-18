using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AMEventParameter {
    public enum ValueType {
        Integer = 0,
        Long = 1,
        Float = 2,
        Double = 3,
        Vector2 = 4,
        Vector3 = 5,
        Vector4 = 6,
        Color = 7,
        Rect = 8,
        String = 9,
        Char = 10,
        Object = 11,
        Array = 12,
        Boolean = 13
    }

    public string paramName;

    public int valueType;

    public int val_int;
    public string val_string;
    public Vector4 val_vect4;
    public UnityEngine.Object val_obj;
    public List<AMEventParameter> lsArray = new List<AMEventParameter>();

    public bool val_bool { get { return val_int == 1; } set { val_int = value ? 1 : 0; } }
    public long val_long { get { return val_int; } set { val_int = (int)value; } }
    public float val_float { get { return val_vect4.x; } set { val_vect4.Set(value, 0,0,0); } }
    public double val_double { get { return val_float; } set { val_float = (float)value; } }
    public Vector2 val_vect2 { get { return new Vector2(val_vect4.x, val_vect4.y); } set { val_vect4.Set(value.x, value.y, 0, 0); } }
    public Vector3 val_vect3 { get { return new Vector3(val_vect4.x, val_vect4.y, val_vect4.z); } set { val_vect4.Set(value.x, value.y, value.z, 0); } }

    public Color val_color { get { return new Color(val_vect4.x, val_vect4.y, val_vect4.z, val_vect4.w); } set { val_vect4.Set(value.r, value.g, value.b, value.a); } }
    public Rect val_rect { get { return new Rect(val_vect4.x, val_vect4.y, val_vect4.z, val_vect4.w); } set { val_vect4.Set(value.xMin, value.yMin, value.width, value.height); } }
    
    public AMEventParameter() {

    }
    public bool setBool(bool val_bool) {
        if(this.val_bool != val_bool) {
            this.val_bool = val_bool;
            return true;
        }
        return false;
    }
    public bool setInt(int val_int) {
        if(this.val_int != val_int) {
            this.val_int = val_int;
            return true;
        }
        return false;
    }
    public bool setLong(int val_long) {
        if(this.val_long != val_long) {
            this.val_long = val_long;
            return true;
        }
        return false;
    }
    public bool setFloat(float val_float) {
        if(this.val_float != val_float) {
            this.val_float = val_float;
            return true;
        }
        return false;
    }
    public bool setDouble(double val_double) {
        if(this.val_double != val_double) {
            this.val_double = val_double;
            return true;
        }
        return false;
    }
    public bool setVector2(Vector2 val_vect2) {
        if(this.val_vect2 != val_vect2) {
            this.val_vect2 = val_vect2;
            return true;
        }
        return false;
    }
    public bool setVector3(Vector3 val_vect3) {
        if(this.val_vect3 != val_vect3) {
            this.val_vect3 = val_vect3;
            return true;
        }
        return false;
    }
    public bool setVector4(Vector4 val_vect4) {
        if(this.val_vect4 != val_vect4) {
            this.val_vect4 = val_vect4;
            return true;
        }
        return false;
    }
    public bool setColor(Color val_color) {
        if(this.val_color != val_color) {
            this.val_color = val_color;
            return true;
        }
        return false;
    }
    public bool setRect(Rect val_rect) {
        if(this.val_rect != val_rect) {
            this.val_rect = val_rect;
            return true;
        }
        return false;
    }
    public bool setString(string val_string) {
        if(this.val_string != val_string) {
            this.val_string = val_string;
            return true;
        }
        return false;
    }
    public bool setObject(UnityEngine.Object val_obj) {
        if(this.val_obj != val_obj) {
            this.val_obj = val_obj;
            return true;
        }
        return false;
    }
    public bool checkArrayIntegrity() {
        if(valueType == (int)ValueType.Array) {
            if(lsArray != null && lsArray.Count > 0) {
                int valElem = lsArray[0].valueType;
                foreach(AMEventParameter elem in lsArray) {
                    if(elem.valueType != valElem)
                        return false;
                }
            }
        }

        return true;
    }
    public Type getParamType() {
        if(valueType == (int)ValueType.Boolean) return typeof(bool);
        if(valueType == (int)ValueType.Integer) return typeof(int);
        if(valueType == (int)ValueType.Long) return typeof(long);
        if(valueType == (int)ValueType.Float) return typeof(float);
        if(valueType == (int)ValueType.Double) return typeof(double);
        if(valueType == (int)ValueType.Vector2) return typeof(Vector2);
        if(valueType == (int)ValueType.Vector3) return typeof(Vector3);
        if(valueType == (int)ValueType.Vector4) return typeof(Vector4);
        if(valueType == (int)ValueType.Color) return typeof(Color);
        if(valueType == (int)ValueType.Rect) return typeof(Rect);
        if(valueType == (int)ValueType.Object) return typeof(UnityEngine.Object);
        if(valueType == (int)ValueType.Array) {
            if(lsArray.Count <= 0) return typeof(object[]);
            else {
                switch((ValueType)lsArray[0].valueType) {
                    case ValueType.Integer:
                        return typeof(int[]);
                    case ValueType.Long:
                        return typeof(long[]);
                    case ValueType.Float:
                        return typeof(float[]);
                    case ValueType.Double:
                        return typeof(double[]);
                    case ValueType.Vector2:
                        return typeof(Vector2[]);
                    case ValueType.Vector3:
                        return typeof(Vector3[]);
                    case ValueType.Vector4:
                        return typeof(Vector4[]);
                    case ValueType.Color:
                        return typeof(Color[]);
                    case ValueType.Rect:
                        return typeof(Rect[]);
                    case ValueType.String:
                        return typeof(string[]);
                    case ValueType.Char:
                        return typeof(char[]);
                    case ValueType.Object:
                        return typeof(UnityEngine.Object[]);
                    case ValueType.Array:
                        return typeof(System.Array[]);
                    case ValueType.Boolean:
                        return typeof(bool[]);
                }
            }
            //else return lsArray[0].getParamType();
        }
        if(valueType == (int)ValueType.String) return typeof(string);
        if(valueType == (int)ValueType.Char) return typeof(char);
        Debug.LogError("Animator: Type not found for Event Parameter.");
        return typeof(object);
    }

    public void destroy() {
        foreach(AMEventParameter param in lsArray)
            param.destroy();
    }

    public string getStringValue() {
        if(valueType == (int)ValueType.Boolean) return val_bool.ToString().ToLower();
        if(valueType == (int)ValueType.String) return "\"" + val_string + "\"";
        if(valueType == (int)ValueType.Char) {
            if(val_string == null || val_string.Length <= 0) return "''";
            else return "'" + val_string[0] + "'";
        }
        if(valueType == (int)ValueType.Integer || valueType == (int)ValueType.Long) return val_int.ToString();
        if(valueType == (int)ValueType.Float || valueType == (int)ValueType.Double) return val_float.ToString();
        if(valueType == (int)ValueType.Vector2) return val_vect2.ToString();
        if(valueType == (int)ValueType.Vector3) return val_vect3.ToString();
        if(valueType == (int)ValueType.Vector4) return val_vect4.ToString();
        if(valueType == (int)ValueType.Color) return val_color.ToString();
        if(valueType == (int)ValueType.Rect) return val_rect.ToString();
        if(valueType == (int)ValueType.Object)
            if(!val_obj) return "None";
            else return val_obj.name;

        if(valueType == (int)ValueType.Array) return "Array";
        Debug.LogError("Animator: Type not found for Event Parameter.");
        return "object";
    }

    public void setValueType(Type t) {
        if(t == typeof(bool)) valueType = (int)ValueType.Boolean;
        else if(t == typeof(string)) valueType = (int)ValueType.String;
        else if(t == typeof(char)) valueType = (int)ValueType.Char;
        else if(t == typeof(int)) valueType = (int)ValueType.Integer;
        else if(t == typeof(long)) valueType = (int)ValueType.Long;
        else if(t == typeof(float)) valueType = (int)ValueType.Float;
        else if(t == typeof(double)) valueType = (int)ValueType.Double;
        else if(t == typeof(Vector2)) valueType = (int)ValueType.Vector2;
        else if(t == typeof(Vector3)) valueType = (int)ValueType.Vector3;
        else if(t == typeof(Vector4)) valueType = (int)ValueType.Vector4;
        else if(t == typeof(Color)) valueType = (int)ValueType.Color;
        else if(t == typeof(Rect)) valueType = (int)ValueType.Rect;
        else if(t.IsArray) valueType = (int)ValueType.Array;
        else if(t.BaseType.BaseType == typeof(UnityEngine.Object)) valueType = (int)ValueType.Object;
        else {
            valueType = (int)ValueType.Object;
        }
    }

    public void fromObject(object dat) {
        switch((ValueType)valueType) {
            case ValueType.Integer:
                val_int = Convert.ToInt32(dat);
                break;
            case ValueType.Long:
                val_long = Convert.ToInt64(dat);
                break;
            case ValueType.Float:
                val_float = Convert.ToSingle(dat);
                break;
            case ValueType.Double:
                val_double = Convert.ToDouble(dat);
                break;
            case ValueType.Vector2:
                val_vect2 = (Vector2)dat;
                break;
            case ValueType.Vector3:
                val_vect3 = (Vector3)dat;
                break;
            case ValueType.Vector4:
                val_vect4 = (Vector4)dat;
                break;
            case ValueType.Color:
                val_color = (Color)dat;
                break;
            case ValueType.Rect:
                val_rect = (Rect)dat;
                break;
            case ValueType.String:
                val_string = Convert.ToString(dat);
                break;
            case ValueType.Char:
                val_string = new string(Convert.ToChar(dat), 1);
                break;
            case ValueType.Object:
                val_obj = (UnityEngine.Object)dat;
                break;
            case ValueType.Array:
                if(lsArray != null) {
                    foreach(AMEventParameter param in lsArray)
                        param.destroy();
                }

                System.Array arr = (System.Array)dat;
                lsArray = new List<AMEventParameter>(arr.Length);
                for(int i = 0; i < arr.Length; i++) {
                    object arrElem = arr.GetValue(i);
                    if(arrElem != null) {
                        AMEventParameter a = new AMEventParameter();
                        a.setValueType(arrElem.GetType());
                        a.fromObject(arrElem);
                        lsArray.Add(a);
                    }
                }
                break;
            case ValueType.Boolean:
                val_bool = Convert.ToBoolean(dat);
                break;
        }
    }

    public object toObject() {
        if(valueType == (int)ValueType.Boolean) return (val_bool/* as object*/);
        if(valueType == (int)ValueType.String) return (val_string/* as object*/);
        if(valueType == (int)ValueType.Char) {
            if(val_string == null || val_string.Length <= 0) return '\0';
            return (val_string[0]/* as object*/);
        }
        if(valueType == (int)ValueType.Integer) return (val_int/* as object*/);
        if(valueType == (int)ValueType.Long) return (val_long/* as object*/);
        if(valueType == (int)ValueType.Float) return (val_float/* as object*/);
        if(valueType == (int)ValueType.Double) return (val_double/* as object*/);
        if(valueType == (int)ValueType.Vector2) return (val_vect2/* as object*/);
        if(valueType == (int)ValueType.Vector3) return (val_vect3/* as object*/);
        if(valueType == (int)ValueType.Vector4) return (val_vect4/* as object*/);
        if(valueType == (int)ValueType.Color) return (val_color/* as object*/);
        if(valueType == (int)ValueType.Rect) return (val_rect/* as object*/);
        if(valueType == (int)ValueType.Object) return (val_obj/* as object*/);
        if(valueType == (int)ValueType.Array && lsArray.Count > 0) {
            if(lsArray[0].valueType == (int)ValueType.Boolean) {
                bool[] arrObj = new bool[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_bool;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.String) {
                string[] arrObj = new string[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_string;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Char) {
                char[] arrObj = new char[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_string[0];
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Integer) {
                int[] arrObj = new int[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_int;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Long) {
                long[] arrObj = new long[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_long;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Float) {
                float[] arrObj = new float[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_float;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Double) {
                double[] arrObj = new double[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_double;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Vector2) {
                Vector2[] arrObj = new Vector2[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_vect2;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Vector3) {
                Vector3[] arrObj = new Vector3[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_vect3;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Vector4) {
                Vector4[] arrObj = new Vector4[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_vect4;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Color) {
                Color[] arrObj = new Color[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_color;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Rect) {
                Rect[] arrObj = new Rect[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_rect;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int)ValueType.Object) {
                UnityEngine.Object[] arrObj = new UnityEngine.Object[lsArray.Count];
                for(int i = 0; i < lsArray.Count; i++) {
                    arrObj[i] = lsArray[i].val_obj;
                }
                return arrObj;
            }

        }
        Debug.LogError("Animator: Type not found for Event Parameter.");
        return null;
    }

    public AnimatorTimeline.JSONEventParameter toJSON() {
        AnimatorTimeline.JSONEventParameter e = new AnimatorTimeline.JSONEventParameter();
        e.valueType = valueType;
        if(valueType == (int)ValueType.Boolean) e.val_bool = val_bool;
        if(valueType == (int)ValueType.String) e.val_string = (val_string/* as object*/);
        if(valueType == (int)ValueType.Char) {
            if(val_string == null || val_string.Length <= 0) e.val_string = "\0";
            e.val_string = "" + val_string[0];
        }
        if(valueType == (int)ValueType.Integer || valueType == (int)ValueType.Long) e.val_int = (val_int/* as object*/);
        if(valueType == (int)ValueType.Float || valueType == (int)ValueType.Double) e.val_float = (val_float/* as object*/);
        if(valueType == (int)ValueType.Vector2) {
            AnimatorTimeline.JSONVector2 v2 = new AnimatorTimeline.JSONVector2();
            v2.setValue(val_vect2);
            e.val_vect2 = v2;
        }
        if(valueType == (int)ValueType.Vector3) {
            AnimatorTimeline.JSONVector3 v3 = new AnimatorTimeline.JSONVector3();
            v3.setValue(val_vect3);
            e.val_vect3 = v3;
        }
        if(valueType == (int)ValueType.Vector4) {
            AnimatorTimeline.JSONVector4 v4 = new AnimatorTimeline.JSONVector4();
            v4.setValue(val_vect4);
            e.val_vect4 = v4;
        }
        if(valueType == (int)ValueType.Color) {
            AnimatorTimeline.JSONColor c = new AnimatorTimeline.JSONColor();
            c.setValue(val_color);
            e.val_color = c;
        }
        if(valueType == (int)ValueType.Rect) {
            AnimatorTimeline.JSONRect r = new AnimatorTimeline.JSONRect();
            r.setValue(val_rect);
            e.val_rect = r;
        }
        if(valueType == (int)ValueType.Object) {
            if(val_obj.GetType() != typeof(GameObject)) {
                // component
                e.val_obj_extra = val_obj.name;
                e.val_obj = val_obj.GetType().Name;
            }
            else {
                // gameobject
                e.val_obj_extra = null;
                e.val_obj = val_obj.name;
            }

        }
        if(valueType == (int)ValueType.Array && lsArray.Count > 0) {
            AnimatorTimeline.JSONEventParameter[] arr = new AnimatorTimeline.JSONEventParameter[lsArray.Count];
            for(int i = 0; i < lsArray.Count; i++) {
                //arrObj[i] = lsArray[i].val_bool;
                arr[i] = lsArray[i].toJSON();
            }
            e.array = arr;
            /*
            if(lsArray[0].valueType == (int) ValueType.Boolean) {
                //bool[] arrObj = new bool[lsArray.Count];
//				AnimatorTimeline.JSONEventParameter[] arr = new AnimatorTimeline.JSONEventParameter[lsArray.Count];
//				for(int i=0;i<lsArray.Count;i++){
//					//arrObj[i] = lsArray[i].val_bool;
//					arr[i] = lsArray[i].toJSON();
//				}
                //return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.String) {
                string[] arrObj = new string[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_string;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Char) {
                char[] arrObj = new char[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_string[0];
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Integer || lsArray[0].valueType == (int) ValueType.Long) {
                int[] arrObj = new int[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_int;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Float || lsArray[0].valueType == (int) ValueType.Double) {
                float[] arrObj = new float[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_float;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Vector2) {
                Vector2[] arrObj = new Vector2[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_vect2;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Vector3) {
                Vector3[] arrObj = new Vector3[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_vect3;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Vector4) {
                Vector4[] arrObj = new Vector4[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_vect4;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Color) {
                Color[] arrObj = new Color[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_color;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Rect) {
                Rect[] arrObj = new Rect[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_rect;
                }
                return arrObj;
            }
            if(lsArray[0].valueType == (int) ValueType.Object) {
                UnityEngine.Object[] arrObj = new UnityEngine.Object[lsArray.Count];
                for(int i=0;i<lsArray.Count;i++){
                    arrObj[i] = lsArray[i].val_obj;
                }
                return arrObj;
            }*/

        }
        //Debug.LogError("Animator: Type not found for Event Parameter.");
        return e;
    }

    public object[] toArray() {
        object[] arr = new object[lsArray.Count];
        for(int i = 0; i < lsArray.Count; i++)
            arr[i] = lsArray[i].toObject();

        return arr;
    }
    public bool isArray() {
        if(lsArray.Count > 0) return true;
        return false;
    }

    public AMEventParameter CreateClone() {
        AMEventParameter a = new AMEventParameter();
        a.valueType = valueType;
        a.val_int = val_int;
        a.val_vect4 = val_vect4;
        a.val_string = val_string;
        a.val_obj = val_obj;
        foreach(AMEventParameter e in lsArray) {
            a.lsArray.Add(e.CreateClone());
        }
        return a;
    }

    public List<GameObject> getDependencies() {
        List<GameObject> ls = new List<GameObject>();
        if(valueType == (int)ValueType.Object && val_obj) {
            if(val_obj is GameObject) ls.Add((GameObject)val_obj);
            else {
                ls.Add((val_obj as Component).gameObject);
            }
        }
        else if(valueType == (int)ValueType.Array) {
            if(lsArray.Count > 0 && lsArray[0].valueType == (int)ValueType.Object) {
                foreach(AMEventParameter param in lsArray) {
                    if(param.val_obj) {
                        if(param.val_obj is GameObject)
                            ls.Add((GameObject)param.val_obj);
                        else {
                            ls.Add((param.val_obj as Component).gameObject);
                        }
                    }
                }
            }
        }
        return ls;
    }

    public bool updateDependencies(List<GameObject> newReferences, List<GameObject> oldReferences) {
        bool didUpdateParameter = false;
        if(valueType == (int)ValueType.Object && val_obj) {
            for(int i = 0; i < oldReferences.Count; i++) {
                if(val_obj is GameObject) {
                    if(oldReferences[i] == val_obj) {
                        val_obj = newReferences[i];
                        didUpdateParameter = true;
                        break;
                    }
                }
                else {
                    if(oldReferences[i] == (val_obj as Component).gameObject) {
                        val_obj = newReferences[i].GetComponent(val_obj.GetType());
                        didUpdateParameter = true;
                        break;
                    }
                }
            }
        }
        else if(valueType == (int)ValueType.Array) {
            if(lsArray.Count > 0 && lsArray[0].valueType == (int)ValueType.Object) {
                for(int i = 0; i < oldReferences.Count; i++) {
                    foreach(AMEventParameter param in lsArray) {
                        if(!param.val_obj) continue;
                        if(param.val_obj is GameObject) {
                            if(oldReferences[i] == param.val_obj) {
                                param.val_obj = newReferences[i];
                                didUpdateParameter = true;
                                break;
                            }
                        }
                        else {
                            if(oldReferences[i] == (param.val_obj as Component).gameObject) {
                                param.val_obj = newReferences[i].GetComponent(param.val_obj.GetType());
                                didUpdateParameter = true;
                                break;
                            }
                        }
                    }

                }

            }
        }
        return didUpdateParameter;
    }
}

using System.Diagnostics;
using System.Globalization;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class VisualStudioFileOpening {
    [OnOpenAsset]
    public static bool OpenFile(int instanceID, int line) {
        var assetPath = AssetDatabase.GetAssetPath(instanceID);

        return CanBeOpened(assetPath)
            && OpenAsset(assetPath, line);
    }

    private static bool CanBeOpened(string assetPath) {
        var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

        return asset is TextAsset || asset is ComputeShader;
    }

    private static bool OpenAsset(string file, int line) {
        const string openerKey = "kScriptsDefaultApp";
        const string openerExe = "UnityVS.OpenFile.exe";

        var openFile = EditorPrefs.GetString(openerKey);

        if(!File.Exists(FullPathTo(file)) || !openFile.EndsWith(openerExe))
            return false;

        Process.Start(new ProcessStartInfo {
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = QuoteIfNeeded(FullPathTo(file)) + " " + (line - 1).ToString(CultureInfo.InvariantCulture),
            FileName = NormalizePath(openFile),
        });

        return true;
    }

    private static string FullPathTo(string file) {
        return Path.GetFullPath(Path.Combine(ProjectDirectory(), NormalizePath(file)));
    }

    private static string NormalizePath(string path) {
        return path.Replace('/', '\\');
    }

    public static string ProjectDirectory() {
        return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    }

    public static string QuoteIfNeeded(string str) {
        return str.Contains(" ") ? "\"" + str + "\"" : str;
    }
}
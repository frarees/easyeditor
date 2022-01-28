namespace EasyEditor
{
    using System.IO;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal static class Preferences
    {
        static Preferences()
        {
            projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            projectName = Path.GetFileName(Path.GetDirectoryName(projectPath));
        }

        public static readonly string projectPath;
        public static readonly string projectName;

        public static bool IsActive => CodeEditor.CurrentEditor is ExternalCodeEditor;

        public static string GetLangVersion()
        {
#if UNITY_2021_2_OR_NEWER
            return "9.0";
#elif UNITY_2020_2_OR_NEWER
            return "8.0";
#else
            return "7.3";
#endif
        }
    }
}


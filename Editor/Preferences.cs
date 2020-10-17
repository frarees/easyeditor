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

#if !UNITY_2020_2_OR_NEWER
        public static string GetLangVersion()
        {
            return "7.3";
        }
#endif
    }
}


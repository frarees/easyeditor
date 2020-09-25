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
            ProjectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            ProjectName = Path.GetFileName(Path.GetDirectoryName(ProjectPath));
        }

        public static string ProjectPath { get; private set; }
        public static string ProjectName { get; private set; }

        public static bool IsActive => CodeEditor.CurrentEditor is ExternalCodeEditor;

        public static string GetLangVersion()
        {
#if UNITY_2020_2_OR_NEWER
            return "8.0";
#else
            return "7.3";
#endif
        }

        public const string prefPrefix = "EasyEditor.";

        private const string prefMatchCompilerVersion = prefPrefix + "MatchCompilerVersion";
        public static bool MatchCompilerVersion
        {
            get => EditorPrefs.GetBool(prefMatchCompilerVersion);
            set => EditorPrefs.SetBool(prefMatchCompilerVersion, value);
        }

        private const string prefAutoSync = prefPrefix + "AutoSync";
        public static bool AutoSync
        {
            get => EditorPrefs.GetBool(prefAutoSync);
            set => EditorPrefs.SetBool(prefAutoSync, value);
        }
    }
}


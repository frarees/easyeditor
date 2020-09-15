namespace EasyEditor
{
    using UnityEngine;
    using UnityEditor;
    using Unity.CodeEditor;
    using System.IO;

    [InitializeOnLoad]
    internal static class Preferences
    {
        private const string prefPrefix = "EasyEditor.";
        private const string prefMatchCompilerVersion = prefPrefix + "MatchCompilerVersion";
        private const string prefAutoSync = prefPrefix + "AutoSync";

        public static string ProjectPath => Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        public static string ProjectName => Path.GetFileName(Path.GetDirectoryName(ProjectPath));

        public static ExternalCodeEditor ExternalCodeEditor { get; private set; }

        public static bool IsActive => CodeEditor.CurrentEditor is ExternalCodeEditor;

        public static bool MatchCompilerVersion
        {
            get => EditorPrefs.GetBool(prefMatchCompilerVersion);
            set => EditorPrefs.SetBool(prefMatchCompilerVersion, value);
        }

        public static bool AutoSync
        {
            get => EditorPrefs.GetBool(prefAutoSync);
            set => EditorPrefs.SetBool(prefAutoSync, value);
        }

        static Preferences()
        {
            ExternalCodeEditor = new ExternalCodeEditor();
            CodeEditor.Register(ExternalCodeEditor);
        }
    }
}


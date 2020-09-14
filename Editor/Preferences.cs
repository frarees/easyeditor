using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.CodeEditor;
using System.IO;

namespace EasyEditor
{
    [InitializeOnLoad]
    internal static class Preferences
    {
        private const string kUseEasyEditor = "kUseEasyEditor";

        public static string ProjectPath => Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        public static string ProjectName => Path.GetFileName(Path.GetDirectoryName(ProjectPath));

        public static ExternalCodeEditor ExternalCodeEditor { get; private set; }

        public static bool UseEasyEditor
        {
            get => EditorPrefs.GetBool(kUseEasyEditor);
            set => EditorPrefs.SetBool(kUseEasyEditor, value);
        }

        static Preferences()
        {
            ExternalCodeEditor = new ExternalCodeEditor();
            if (UseEasyEditor)
            {
                CodeEditor.Register(ExternalCodeEditor);
            }
        }

        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            return new SettingsProvider("Preferences/Easy Editor", SettingsScope.User)
            {
                label = "Easy Editor",
                guiHandler = GUIHandler,
                keywords = new HashSet<string>(new[] { "Editor" })
            };
        }

        private static void GUIHandler(string searchContext)
        {
            EditorGUI.BeginChangeCheck();
            bool v = EditorGUILayout.Toggle("Use Easy Editor", UseEasyEditor);
            if (EditorGUI.EndChangeCheck())
            {
                UseEasyEditor = v;
                if (v)
                {
                    CodeEditor.Register(ExternalCodeEditor);
                }
                else
                {
                    CodeEditor.Unregister(ExternalCodeEditor);
                    CodeEditor.SetExternalScriptEditor(string.Empty);
                }
                Event.current.Use();
            }

            EditorGUI.BeginDisabledGroup(!UseEasyEditor);
            EditorGUILayout.HelpBox("Easy Editor overrides the behavior of the external script editor selector.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            ExternalCodeEditor.OnGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
}


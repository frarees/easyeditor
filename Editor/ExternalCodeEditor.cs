namespace EasyEditor
{
    using EasyEditor.Reflected;
    using System.IO;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal class ExternalCodeEditor : IExternalCodeEditor
    {
        public static readonly ExternalCodeEditor instance;

        static ExternalCodeEditor()
        {
            LauncherRegistry.Load();
            instance = new ExternalCodeEditor();
            CodeEditor.Register(instance);
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        public CodeEditor.Installation[] Installations { get; }

        private static string[] DefaultExtensions
        {
            get
            {
                string[] customExtensions = new[] { "json", "asmdef", "log" };
                return EditorSettings.projectGenerationBuiltinExtensions
                    .Concat(EditorSettings.projectGenerationUserExtensions)
                    .Concat(customExtensions)
                    .Distinct().ToArray();
            }
        }

        private static bool SupportsExtension(string path)
        {
            string extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) && DefaultExtensions.Contains(extension.TrimStart('.'));
        }

        private static void OnBeforeAssemblyReload()
        {
            CodeEditor.Unregister(instance);
        }

        public ExternalCodeEditor()
        {
            Installations = LauncherRegistry.Installations;
        }

        public void Initialize(string editorInstallationPath)
        {
            if (Preferences.IsActive && Preferences.Settings.autoSync.GetBool())
            {
                SyncUtil.Sync();
            }
        }

        public void OnGUI()
        {
            if (!string.IsNullOrEmpty(LauncherRegistry.LoadErrors))
            {
                EditorGUILayout.HelpBox(LauncherRegistry.LoadErrors, MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(!Preferences.IsActive || !SyncVS.IsValid || SyncUtil.IsReloading || EditorApplication.isCompiling || EditorApplication.isUpdating);
            {
                EditorGUI.BeginChangeCheck();
                Setting s = Preferences.Settings.autoSync;
                GUIContent c = new GUIContent(s.description, s.tooltip);
                bool v = EditorGUILayout.Toggle(c, s.GetBool());
                if (EditorGUI.EndChangeCheck())
                {
                    s.SetBool(v);
                    if (v)
                    {
                        SyncUtil.Sync();
                    }
                    Event.current.Use();
                }
            }
            if (!SyncVS.IsValid)
            {
                EditorGUILayout.HelpBox("Couldn't retrieve synchronization members. Please contact this package's author.", MessageType.Warning);
            }

            {
                EditorGUI.BeginChangeCheck();
                Setting s = Preferences.Settings.matchCompilerVersion;
                GUIContent c = new GUIContent(s.description, s.tooltip);
                bool v = EditorGUILayout.Toggle(c, s.GetBool());
                if (EditorGUI.EndChangeCheck())
                {
                    s.SetBool(v);
                    if (v)
                    {
                        SyncUtil.Sync();
                    }
                    Event.current.Use();
                }
            }

            string editorPath = CodeEditor.CurrentEditorInstallation.Trim();
            Launcher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                launcher.OnGUI();
            }

            EditorGUI.EndDisabledGroup();
        }

        public bool OpenProject(string filePath = "", int line = -1, int column = -1)
        {
            if (!string.IsNullOrEmpty(filePath) && !SupportsExtension(filePath))
            {
                return false;
            }

            if (line <= 0)
            {
                line = 1;
            }

            if (column <= 0)
            {
                column = 1;
            }

            string editorPath = CodeEditor.CurrentEditorInstallation.Trim();

            LaunchDescriptor descriptor = new LaunchDescriptor(filePath, line, column, Preferences.projectPath, Preferences.projectName);

            Launcher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                _ = launcher.Launch(editorPath, descriptor);
            }

            return true;
        }

        public void SyncAll()
        {
            SyncUtil.Sync();
            string editorPath = CodeEditor.CurrentEditorInstallation.Trim();
            Launcher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                launcher.SyncAll();
            }
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            SyncUtil.Sync();
            string editorPath = CodeEditor.CurrentEditorInstallation.Trim();
            Launcher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                launcher.SyncIfNeeded(addedFiles, deletedFiles, movedFiles, movedFromFiles, importedFiles);
            }
        }

        public bool TryGetInstallationForPath(string editorPath, out CodeEditor.Installation installation)
        {
            foreach (CodeEditor.Installation i in Installations)
            {
                if (i.Path == editorPath)
                {
                    installation = i;
                    return true;
                }
            }

            installation = default;
            return false;
        }
    }
}


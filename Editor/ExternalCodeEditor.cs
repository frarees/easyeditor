namespace EasyEditor
{
    using System.IO;
    using Unity.CodeEditor;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    internal class ExternalCodeEditor : IExternalCodeEditor
    {
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

        public ExternalCodeEditor()
        {
            Installations = LauncherRegistry.Installations;
        }

        public void Initialize(string editorInstallationPath)
        {
            if (Preferences.IsActive && Preferences.AutoSync)
            {
                SyncUtil.Sync();
            }
        }

        public void OnGUI()
        {
            if (!SyncUtil.SyncVS.IsValid)
            {
                EditorGUILayout.HelpBox("Couldn't retrieve synchronization members. Please contact this package's author.", MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(!Preferences.IsActive || !SyncUtil.SyncVS.IsValid || SyncUtil.IsReloading || EditorApplication.isCompiling || EditorApplication.isUpdating);
            EditorGUI.BeginChangeCheck();
            bool v = EditorGUILayout.Toggle("Sync solution and project files", Preferences.AutoSync);
            if (EditorGUI.EndChangeCheck())
            {
                Preferences.AutoSync = v;
                if (v)
                {
                    SyncUtil.Sync();
                }
                Event.current.Use();
            }

            EditorGUI.BeginChangeCheck();
            v = EditorGUILayout.Toggle("Match compiler version", Preferences.MatchCompilerVersion);
            if (EditorGUI.EndChangeCheck())
            {
                Preferences.MatchCompilerVersion = v;
                if (v)
                {
                    SyncUtil.Sync();
                }
                Event.current.Use();
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
            CodeEditor.Installation installation = Installations.FirstOrDefault(i => i.Path == editorPath);

            LaunchDescriptor descriptor = new LaunchDescriptor(filePath, line, column, Preferences.ProjectPath, Preferences.ProjectName);

            ILauncher launcher = LauncherRegistry.GetLauncher(editorPath);
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
            ILauncher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                launcher.SyncAll();
            }
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            SyncUtil.Sync();
            string editorPath = CodeEditor.CurrentEditorInstallation.Trim();
            ILauncher launcher = LauncherRegistry.GetLauncher(editorPath);
            if (launcher != null)
            {
                launcher.SyncAll();
            }
        }

        public bool TryGetInstallationForPath(string editorPath, out CodeEditor.Installation installation)
        {
            installation = new CodeEditor.Installation
            {
                Name = Path.GetFileNameWithoutExtension(editorPath),
                Path = editorPath
            };
            return Installations.Any(i => i.Path == editorPath);
        }
    }
}


namespace EasyEditor
{
    using EasyEditor.Reflected;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal class ExternalCodeEditor : IExternalCodeEditor
    {
        private class Properties
        {
            public static readonly GUIContent arguments = EditorGUIUtility.TrTextContent("Arguments");
            public static readonly GUIContent reset = EditorGUIUtility.TrTextContent("Reset");
            public static readonly GUIContent generate = EditorGUIUtility.TrTextContent("Generate");
            public static readonly GUIContent autoGenerate = EditorGUIUtility.TrTextContent("Generate solution and project files", "Forces .sln and .csproj files to be generated and kept in sync.");
#if !UNITY_2020_2_OR_NEWER 
            public static readonly GUIContent matchCompilerVersion = EditorGUIUtility.TrTextContent("Match compiler version", "When Unity creates or updates .csproj files, it defines LangVersion as 'latest'. This can create inconsistencies with other .NET platforms (e.g. OmniSharp), which could resolve 'latest' as a different version. By matching compiler version, 'latest' will get resolved as " + Preferences.GetLangVersion() + ". ");
#endif
            public static readonly GUIContent exportFrameworkPathOverride = EditorGUIUtility.TrTextContent("Export FrameworkPathOverride", FrameworkResolver.LastAvailableFrameworkPath != null ? "When invoking the text editor, sets the environment variable FrameworkPathOverride to " + FrameworkResolver.LastAvailableFrameworkPath : string.Empty);

            public static readonly GUIContent syncVsFail = EditorGUIUtility.TrTextContent("Couldn't reflect SyncVS members successfully.");
            public static readonly GUIContent monoInstallationFinderFail = EditorGUIUtility.TrTextContent("Couldn't reflect MonoInstallationFinder members successfully.");
            public static readonly GUIContent discoveryFail = EditorGUIUtility.TrTextContent("Easy Editor couldn't load the discovery.");
        }

        public static readonly ExternalCodeEditor instance;
        private static IEnumerable<string> DefaultExtensions { get; }

        public CodeEditor.Installation[] Installations { get; }

        static ExternalCodeEditor()
        {
            DefaultExtensions = EditorSettings.projectGenerationBuiltinExtensions
                    .Concat(EditorSettings.projectGenerationUserExtensions)
                    .Concat(new[] { "json", "asmdef", "log" })
                    .Distinct();

            instance = new ExternalCodeEditor();
            CodeEditor.Register(instance);

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static bool SupportsExtension(string path)
        {
            string extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) && DefaultExtensions.Contains(extension.TrimStart('.'));
        }

        private static void OnAfterAssemblyReload()
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    SyncUtil.Sync();
                }
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            CodeEditor.Unregister(instance);
        }

        private ExternalCodeEditor()
        {
            if (Registry.LoadInstance())
            {
                Installations = Registry.Instance.GetInstallations();
            }
        }

        public void Initialize(string editorInstallationPath)
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    SyncUtil.Sync();
                }
            }
        }

        public void OnGUI()
        {
            string editorPath = CodeEditor.CurrentEditorInstallation;
            if (!Registry.Instance.TryGetDiscoveryFromEditorPath(editorPath, out Discovery discovery))
            {
                EditorGUILayout.HelpBox(Properties.discoveryFail.text, MessageType.Error);
                return;
            }

            EditorGUI.BeginDisabledGroup(!Preferences.IsActive || SyncUtil.IsReloading || EditorApplication.isCompiling || EditorApplication.isUpdating);

            if (!string.IsNullOrEmpty(discovery.notes))
            {
                EditorGUILayout.HelpBox(discovery.notes, MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string arguments = EditorGUILayout.TextField(Properties.arguments, discovery.Arguments);
            if (EditorGUI.EndChangeCheck())
            {
                discovery.Arguments = arguments;
            }

            if (GUILayout.Button(Properties.reset, EditorStyles.miniButton, GUILayout.Width(64f)))
            {
                discovery.Arguments = discovery.defaultArguments;
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!SyncVS.IsValid);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            bool autoGenerate = EditorGUILayout.Toggle(Properties.autoGenerate, discovery.AutoGenerate);
            if (EditorGUI.EndChangeCheck())
            {
                discovery.AutoGenerate = autoGenerate;
                if (autoGenerate)
                {
                    SyncUtil.Sync();
                }
            }

            if (GUILayout.Button(Properties.generate, EditorStyles.miniButton, GUILayout.Width(64f)))
            {
                SyncUtil.Sync();
            }

            EditorGUILayout.EndHorizontal();

#if !UNITY_2020_2_OR_NEWER
            EditorGUI.BeginChangeCheck();
            bool matchCompilerVersion = EditorGUILayout.Toggle(Properties.matchCompilerVersion, discovery.MatchCompilerVersion);
            if (EditorGUI.EndChangeCheck())
            {
                discovery.MatchCompilerVersion = matchCompilerVersion;
                if (matchCompilerVersion)
                {
                    SyncUtil.Sync();
                }
            }
#endif
            EditorGUI.EndDisabledGroup();

            if (!SyncVS.IsValid)
            {
                EditorGUILayout.HelpBox(Properties.syncVsFail.text, MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(!MonoInstallationFinder.IsValid || !discovery.inheritsEnvironmentVariables);

            if (discovery.inheritsEnvironmentVariables)
            {
                EditorGUI.BeginChangeCheck();
                bool v = EditorGUILayout.Toggle(Properties.exportFrameworkPathOverride, discovery.ExportFrameworkPathOverride);
                if (EditorGUI.EndChangeCheck())
                {
                    discovery.ExportFrameworkPathOverride = v;
                }
            }

            EditorGUI.EndDisabledGroup();

            if (!MonoInstallationFinder.IsValid)
            {
                EditorGUILayout.HelpBox(Properties.monoInstallationFinderFail.text, MessageType.Warning);
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
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(editorPath, out Discovery discovery))
            {
                string arguments = ParseArguments(discovery.Arguments, filePath, line, column);
                bool success = Launch(editorPath, arguments, discovery);

                if (!success)
                {
                    UnityEngine.Debug.LogWarning($"Failed invoking `{editorPath} {arguments}`");
                }

                return true;
            }

            return false;
        }

        private string ParseArguments(string arguments, string filePath, int line, int column)
        {
            arguments = arguments.Replace("$(ProjectName)", Preferences.projectName);
            arguments = CodeEditor.ParseArgument(arguments, filePath, line, column);

            return arguments;
        }

        private bool Launch(string editorPath, string resolvedArguments, Discovery discovery)
        {
            if (discovery.requiresNativeOpen)
            {
                return CodeEditor.OSOpenFile(editorPath, resolvedArguments);
            }

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    Arguments = resolvedArguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };

            if (discovery.ExportFrameworkPathOverride)
            {
                process.StartInfo.EnvironmentVariables.Add("FrameworkPathOverride", FrameworkResolver.AvailableFrameworkPaths.LastOrDefault());
            }

            return process.Start();
        }

        public void SyncAll()
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    SyncUtil.Sync();
                }
            }
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    SyncUtil.Sync();
                }
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


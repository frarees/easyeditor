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
            public static readonly GUIContent clear = EditorGUIUtility.TrTextContent("Clear", "Remove all .sln and .csproj files");
            public static readonly GUIContent autoGenerate = EditorGUIUtility.TrTextContent("Generate solution and project files", "Forces .sln and .csproj files to be generated and kept in sync.");
            public static readonly GUIContent matchCompilerVersion = EditorGUIUtility.TrTextContent("Match compiler version", "When Unity creates or updates .csproj files, it defines LangVersion as 'latest'. This can create inconsistencies with other .NET platforms (e.g. OmniSharp), which could resolve 'latest' as a different version. By matching compiler version, 'latest' will get resolved as " + Preferences.GetLangVersion() + ". ");
            public static readonly GUIContent exportFrameworkPathOverride = EditorGUIUtility.TrTextContent("Export FrameworkPathOverride", FrameworkResolver.LastAvailableFrameworkPath != null ? "When invoking the text editor, sets the environment variable FrameworkPathOverride to " + FrameworkResolver.LastAvailableFrameworkPath : string.Empty);

            public static readonly GUIContent monoInstallationFinderFail = EditorGUIUtility.TrTextContent("Couldn't reflect MonoInstallationFinder members successfully.");
            public static readonly GUIContent discoveryFail = EditorGUIUtility.TrTextContent("Easy Editor couldn't load the discovery.");
        }

        public static ExternalCodeEditor Instance { get; private set; }

        private static IEnumerable<string> DefaultExtensions => EditorSettings.projectGenerationBuiltinExtensions
            .Concat(EditorSettings.projectGenerationUserExtensions)
            .Concat(new[] { "json", "asmdef", "log" })
            .Distinct();

        public CodeEditor.Installation[] Installations { get; }

        private readonly IGenerator generator = new ProjectGeneration(Directory.GetParent(Application.dataPath).FullName);

        static ExternalCodeEditor()
        {
            EditorApplication.delayCall += RegisterInstance;

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void RegisterInstance()
        {
            Instance = new ExternalCodeEditor();

            if (Instance != null)
            {
                CodeEditor.Register(Instance);
            }
        }

        private static bool SupportsExtension(string path)
        {
            string extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) && DefaultExtensions.Contains(extension.TrimStart('.'));
        }

        private static void OnAfterAssemblyReload()
        {
            if (!Registry.Instance)
            {
                return;
            }

            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    if (CodeEditor.CurrentEditor == Instance)
                    {
                        CodeEditor.CurrentEditor.SyncAll();
                    }
                }
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            if (Instance != null)
            {
                CodeEditor.Unregister(Instance);
            }
        }

        private ExternalCodeEditor()
        {
            Installations = Registry.LoadInstance() ? Registry.Instance.GetInstallations() : new CodeEditor.Installation[] { };
        }

        public void Initialize(string editorInstallationPath)
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    generator.Sync();
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

            EditorGUI.BeginDisabledGroup(!Preferences.IsActive || EditorApplication.isCompiling || EditorApplication.isUpdating);

            if (!string.IsNullOrEmpty(discovery.notes))
            {
                EditorGUILayout.HelpBox(discovery.notes, MessageType.Info);
            }

            _ = EditorGUILayout.BeginHorizontal();
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

            _ = EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            bool autoGenerate = EditorGUILayout.Toggle(Properties.autoGenerate, discovery.AutoGenerate);
            if (EditorGUI.EndChangeCheck())
            {
                discovery.AutoGenerate = autoGenerate;
                if (autoGenerate)
                {
                    generator.Sync();
                }
            }

            var flags = generator.AssemblyNameProvider.ProjectGenerationFlag;
            EditorGUI.BeginChangeCheck();
            var nflags = (ProjectGenerationFlag)EditorGUILayout.EnumFlagsField(flags);
            if (EditorGUI.EndChangeCheck())
            {
                generator.AssemblyNameProvider.ToggleProjectGeneration(nflags ^ flags);
            }

            if (GUILayout.Button(Properties.generate, EditorStyles.miniButtonLeft, GUILayout.Width(64f)))
            {
                generator.Sync();
            }

            if (GUILayout.Button(Properties.clear, EditorStyles.miniButtonRight, GUILayout.Width(40f)))
            {
                var di = new DirectoryInfo(Preferences.projectPath);
                foreach (var fi in di.EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly))
                {
                    fi.Delete();
                }

                foreach (var fi in di.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly))
                {
                    fi.Delete();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            bool matchCompilerVersion = EditorGUILayout.Toggle(Properties.matchCompilerVersion, discovery.MatchCompilerVersion);
            if (EditorGUI.EndChangeCheck())
            {
                discovery.MatchCompilerVersion = matchCompilerVersion;
                if (matchCompilerVersion)
                {
                    generator.Sync();
                }
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
                    (generator.AssemblyNameProvider as IPackageInfoCache)?.ResetPackageInfoCache();
                    AssetDatabase.Refresh();
                    generator.Sync();
                }
            }
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.AutoGenerate)
                {
                    (generator.AssemblyNameProvider as IPackageInfoCache)?.ResetPackageInfoCache();
                    _ = generator.SyncIfNeeded(addedFiles.Union(deletedFiles).Union(movedFiles).Union(movedFromFiles).ToList(), importedFiles);
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


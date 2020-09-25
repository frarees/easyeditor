namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEngine;
    using UnityEditor;

    internal class MacVim : ILauncher
    {
        private static readonly string PrefPrefix = $"{typeof(MacVim).FullName}";
        private static readonly string PrefRestartOmniSharp = $"{PrefPrefix}.RestartOmniSharp";

        public CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
#if UNITY_EDITOR_OSX
            new CodeEditor.Installation()
            {
                Name = "MacVim",
                Path = "/usr/local/bin/mvim",
            },
#endif
        };

        public bool Launch(string editorPath, LaunchDescriptor descriptor)
        {
            string args = string.Format("--servername \"{0}\" --remote-silent '+call cursor($(Line),$(Column))' $(File)", descriptor.ProjectName);

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    Arguments = CodeEditor.ParseArgument(args, descriptor.FilePath, descriptor.Line, descriptor.Column),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };

            process.StartInfo.EnvironmentVariables.Add("FrameworkPathOverride", FrameworkResolver.AvailableFrameworkPaths.LastOrDefault());

            return process.Start();
        }

        private void ReloadOmniSharpServer()
        {
            string args = string.Format("--servername \"{0}\" --remote-send '<C-\\><C-N>:silent OmniSharpRestartServer<CR>'", Preferences.ProjectName);

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = CodeEditor.CurrentEditorInstallation.Trim(),
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };

            _ = process.Start();
        }

        public void SyncAll()
        {
            if (Preferences.IsActive && EditorPrefs.GetBool(PrefRestartOmniSharp))
            {
                ReloadOmniSharpServer();
            }
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            if (Preferences.IsActive && EditorPrefs.GetBool(PrefRestartOmniSharp))
            {
                ReloadOmniSharpServer();
            }
        }

        public bool MatchesExecutable(string editorPath)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    Arguments = "--version",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            };

            if (!process.Start())
            {
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            return output.StartsWith("VIM - Vi IMproved");
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUIContent matchCompilerContent = new GUIContent(
                "Restart OmniSharp server on sync",
                "Upon synchronization, send a command (--remote-send) to the Vim server to trigger a OmniSharp server reload. omnisharp-vim required.");
            bool b = EditorGUILayout.Toggle(matchCompilerContent, EditorPrefs.GetBool(PrefRestartOmniSharp));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(PrefRestartOmniSharp, b);
                Event.current.Use();
            }
        }
    }
}


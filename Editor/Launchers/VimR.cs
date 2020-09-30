namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    internal class VimR : ILauncher
    {
        private static readonly string PrefPrefix = $"{typeof(VimR).FullName}";
        private static readonly string PrefRestartOmniSharp = $"{PrefPrefix}.RestartOmniSharp";

        public CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
#if UNITY_EDITOR_OSX
            new CodeEditor.Installation()
            {
                Name = "VimR",
                Path = "/usr/local/bin/vimr",
            },
#endif
        };

        public bool Launch(string editorPath, LaunchDescriptor descriptor)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    Arguments = CodeEditor.ParseArgument("--line $(Line) $(File)", descriptor.FilePath, descriptor.Line, descriptor.Column),
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
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = CodeEditor.CurrentEditorInstallation.Trim(),
                    Arguments = "-c OmniSharpRestartServer",
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
                    Arguments = "--help",
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
            return output.StartsWith("usage: vimr");
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUIContent restartOmniSharpContent = new GUIContent(
                "Restart OmniSharp server on sync",
                "Upon synchronization, execute a command (-c) on VimR's neovim instance to trigger a OmniSharp server reload. omnisharp-vim required.");
            bool b = EditorGUILayout.Toggle(restartOmniSharpContent, EditorPrefs.GetBool(PrefRestartOmniSharp));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(PrefRestartOmniSharp, b);
                Event.current.Use();
            }
        }
    }
}


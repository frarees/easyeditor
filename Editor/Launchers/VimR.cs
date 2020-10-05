namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    internal class VimR : Launcher
    {
        [InitializeOnLoad]
        private static class Settings
        {
            public static readonly Setting restartOmniSharp = new Setting(
                "RestartOmniSharp",
                "Restart OmniSharp server on sync",
                "Upon synchronization, execute a command (--cmd) on a new neovim instance (--nvim) to trigger a OmniSharp server reload. omnisharp-vim required.",
                "VimR");
        }

        public override CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
#if UNITY_EDITOR_OSX
            new CodeEditor.Installation()
            {
                Name = "VimR",
                Path = "/usr/local/bin/vimr",
            },
#endif
        };

        public override bool Launch(string editorPath, LaunchDescriptor descriptor)
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
                    Arguments = "--nvim --cmd OmniSharpRestartServer",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };

            _ = process.Start();
        }

        public override void SyncAll()
        {
            if (Preferences.IsActive && Settings.restartOmniSharp.GetBool())
            {
                ReloadOmniSharpServer();
            }
        }

        public override void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            if (Preferences.IsActive && Settings.restartOmniSharp.GetBool())
            {
                ReloadOmniSharpServer();
            }
        }

        public override bool MatchesExecutable(string editorPath)
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

        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            Setting s = Settings.restartOmniSharp;
            GUIContent c = new GUIContent(s.description, s.tooltip);
            bool b = EditorGUILayout.Toggle(c, s.GetBool());
            if (EditorGUI.EndChangeCheck())
            {
                s.SetBool(b);
                Event.current.Use();
            }
        }
    }
}


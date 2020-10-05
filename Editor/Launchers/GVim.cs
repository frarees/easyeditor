namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    internal class GVim : Launcher
    {
        [InitializeOnLoad]
        private static class Settings
        {
            public static readonly Setting restartOmniSharp = new Setting(
                "RestartOmniSharp",
                "Restart OmniSharp server on sync",
                "Upon synchronization, send a command (--remote-send) to the gVim server to trigger a OmniSharp server reload. omnisharp-vim required.",
                "GVim");
        }

        public override CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
#if UNITY_EDITOR_WIN
            new CodeEditor.Installation()
            {
                Name = "gVim 8.2",
                Path = @"C:\Program Files (x86)\Vim\vim82\gvim.exe",
            },
            new CodeEditor.Installation()
            {
                Name = "gVim 8.1",
                Path = @"C:\Program Files (x86)\Vim\vim81\gvim.exe",
            },
#endif
        };

        public override bool Launch(string editorPath, LaunchDescriptor descriptor)
        {
            string args = string.Format("--servername \"{0}\" --remote-silent +$(Line) $(File)", descriptor.ProjectName);

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
            string args = string.Format("--servername \"{0}\" --remote-send '<C-\\><C-N>:silent OmniSharpRestartServer<CR>'", Preferences.projectName);

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
            string vimPath = Path.Combine(Path.GetDirectoryName(editorPath), "vim.exe");
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = vimPath,
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


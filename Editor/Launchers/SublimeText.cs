namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    internal class SublimeText : Launcher
    {
        [InitializeOnLoad]
        private static class Settings
        {
            public static readonly Setting restartOmniSharp = new Setting(
                "RestartOmniSharp",
                "Restart OmniSharp server on sync",
                "Upon synchronization, send a command (--command) to Sublime Text to trigger a OmniSharp server reload. omnisharp-sublime required.",
                "SublimeText");
        }

        public override CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
#if UNITY_EDITOR_OSX
            new CodeEditor.Installation()
            {
                Name = "Sublime Text",
                Path = "/Applications/Sublime Text.app/Contents/SharedSupport/bin/subl",
            },
#elif UNITY_EDITOR_WIN
            new CodeEditor.Installation()
            {
                Name = "Sublime Text 3",
                Path = @"C:\Program Files\Sublime Text 3\subl.exe"
            },
#endif
        };

        public override bool Launch(string editorPath, LaunchDescriptor descriptor)
        {
            string args = string.Format("$(File):$(Line):$(Column)", descriptor.ProjectName);

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
            string args = "-b --command OmniSharpRestartServer";

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
            return output.StartsWith("Sublime Text");
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


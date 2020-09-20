namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEngine;
    using UnityEditor;

    internal class SublimeText : ILauncher
    {
        private static readonly string PrefPrefix = $"{typeof(SublimeText).FullName}";
        private static readonly string PrefRestartOmniSharp = $"{PrefPrefix}.RestartOmniSharp";

        public CodeEditor.Installation[] Installations => new CodeEditor.Installation[]
        {
            new CodeEditor.Installation()
            {
                Name = "Sublime Text",
                Path = "/Applications/Sublime Text.app/Contents/SharedSupport/bin/subl",
            },
        };

        public bool Launch(string editorPath, LaunchDescriptor descriptor)
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
            return output.StartsWith("Sublime Text");
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUIContent matchCompilerContent = new GUIContent(
                "Restart OmniSharp server on sync",
                "Upon synchronization, send a command (--command) to Sublime Text to trigger a OmniSharp server reload. omnisharp-sublime required.");
            bool b = EditorGUILayout.Toggle(matchCompilerContent, EditorPrefs.GetBool(PrefRestartOmniSharp));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(PrefRestartOmniSharp, b);
                Event.current.Use();
            }
        }
    }
}


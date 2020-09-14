using System.Diagnostics;
using System.Linq;
using Unity.CodeEditor;

namespace EasyEditor.Launchers
{
    [Launcher("MacVim")]
    internal class Vim : ILauncher
    {
        public string[] InstallationPaths => new string[]
        {
            "/usr/local/bin/mvim",
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
            ReloadOmniSharpServer();
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
            ReloadOmniSharpServer();
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
    }
}


namespace EasyEditor.Launchers
{
    using System.Diagnostics;
    using System.Linq;
    using Unity.CodeEditor;

    [Launcher("Sublime Text")]
    internal class SublimeText : ILauncher
    {
        public string[] InstallationPaths => new string[]
        {
            "/Applications/Sublime Text.app/Contents/SharedSupport/bin/subl",
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

        public void SyncAll()
        {
        }

        public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
        {
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
    }
}


// https://forum.unity.com/threads/support-roslyn-analyzers-in-unity-projects.927437

namespace EasyEditor
{
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal static class RoslynDirectoryCreator
    {
        static RoslynDirectoryCreator() => Application.logMessageReceivedThreaded += OnLogMessageReceived;

        private static void OnLogMessageReceived(string message, string stackTrace, LogType logType)
        {
            if (logType != LogType.Exception)
            {
                return;
            }

            const string pattern =
                        @"^DirectoryNotFoundException: Could not find " +
                        @"a part of the path ('|"")Temp(\\|/)RoslynAnalysisRunner";

            if (Regex.IsMatch(message, pattern))
            {
                _ = Directory.CreateDirectory("Temp/RoslynAnalysisRunner");
            }
        }
    }
}

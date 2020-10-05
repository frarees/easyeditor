namespace EasyEditor
{
    using System.IO;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal static class Preferences
    {
        [InitializeOnLoad]
        public static class Settings
        {
            public static readonly Setting autoSync;
            public static readonly Setting matchCompilerVersion;

            static Settings()
            {
                autoSync = new Setting(
                    "AutoSync",
                    "Sync solution and project files",
                    "Forces .sln and .csproj files to be generated and kept in sync.");
                matchCompilerVersion = new Setting(
                    "MatchCompilerVersion",
                    "Match compiler version",
                    "When Unity creates or updates .csproj files, it defines LangVersion as 'latest'. This can create inconsistencies with other .NET platforms (e.g. OmniSharp), which could resolve 'latest' as a different version. By matching compiler version, 'latest' will get resolved as " + GetLangVersion() + ". ");
            }
        }

        static Preferences()
        {
            projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            projectName = Path.GetFileName(Path.GetDirectoryName(projectPath));
        }

        public static readonly string projectPath;
        public static readonly string projectName;

        public static bool IsActive => CodeEditor.CurrentEditor is ExternalCodeEditor;

        public static string GetLangVersion()
        {
#if UNITY_2020_2_OR_NEWER
            return "8.0";
#else
            return "7.3";
#endif
        }
    }
}


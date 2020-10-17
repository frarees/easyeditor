namespace EasyEditor
{
    using EasyEditor.Reflected;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    [InitializeOnLoad]
    internal static class FrameworkResolver
    {
        public static string[] AvailableFrameworkPaths { get; }
        public static string LastAvailableFrameworkPath { get; }
        private static readonly string RootPath;

        static FrameworkResolver()
        {
            if (!MonoInstallationFinder.IsValid)
            {
                return;
            }

            RootPath = MonoInstallationFinder.GetProfilesDirectory(MonoInstallationFinder.MonoBleedingEdgeInstallation);
            AvailableFrameworkPaths = GetAvailableFrameworkPaths(RootPath).ToArray();
            LastAvailableFrameworkPath = AvailableFrameworkPaths.LastOrDefault();
        }

        private static IEnumerable<string> GetAvailableFrameworkPaths(string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                if (dir.EndsWith("-api", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    yield return dir;
                }
            }
        }
    }
}


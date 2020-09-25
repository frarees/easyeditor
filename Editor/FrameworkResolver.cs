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
        private static string RootPath => MonoInstallationFinder.GetProfilesDirectory(MonoInstallationFinder.MonoBleedingEdgeInstallation);

        static FrameworkResolver()
        {
            AvailableFrameworkPaths = GetAvailableFrameworkPaths().ToArray();
        }

        private static IEnumerable<string> GetAvailableFrameworkPaths()
        {
            foreach (string dir in Directory.GetDirectories(RootPath))
            {
                if (dir.EndsWith("-api", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    yield return dir;
                }
            }
        }
    }
}


using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace EasyEditor
{
    [InitializeOnLoad]
    internal static class FrameworkResolver
    {
        public static string[] AvailableFrameworkPaths { get; }
        // TODO add Windows and Linux support
        private static string RootPath => EditorApplication.applicationPath + "/Contents/MonoBleedingEdge/lib/mono";

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


namespace EasyEditor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Unity.CodeEditor;
    using UnityEngine;

    internal static class LauncherRegistry
    {
        public static CodeEditor.Installation[] Installations { get; set; }
        public static string LoadErrors { get; private set; }
        private static readonly Dictionary<string, Launcher> launchers = new Dictionary<string, Launcher>();

        static LauncherRegistry()
        {
        }

        public static Launcher GetLauncher(string editorPath)
        {
            _ = launchers.TryGetValue(editorPath, out Launcher launcher);
            return launcher;
        }

        public static void Load()
        {
            launchers.Clear();
            LoadErrors = string.Empty;
            List<CodeEditor.Installation> installations = new List<CodeEditor.Installation>();
            Assembly assembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "EasyEditor.Launchers");
            Debug.Assert(assembly != null, "Couldn't find the EasyEditor.Launchers assembly");

            foreach (System.Type type in assembly.GetTypes().Where(t => t.IsClass && typeof(Launcher).IsAssignableFrom(t)))
            {
                Launcher instance = (Launcher)System.Activator.CreateInstance(type);

                if (instance.Installations == null)
                {
                    continue;
                }

                foreach (CodeEditor.Installation installation in instance.Installations)
                {
                    if (!File.Exists(installation.Path))
                    {
                        continue;
                    }

                    if (!instance.MatchesExecutable(installation.Path))
                    {
                        LoadErrors += $"{installation.Name}: failed to verify executable {installation.Path}.\n";
                        continue;
                    }

                    installations.Add(installation);
                    launchers.Add(installation.Path, instance);
                }
            }

            Installations = installations.ToArray();
        }
    }
}


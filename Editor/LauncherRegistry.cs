namespace EasyEditor
{
    using UnityEditor;
    using System.Reflection;
    using System.Linq;
    using Unity.CodeEditor;
    using System.Collections.Generic;

    [InitializeOnLoad]
    internal static class LauncherRegistry
    {
        public static CodeEditor.Installation[] Installations { get; set; }
        public static string LoadErrors { get; private set; }
        private static readonly Dictionary<string, ILauncher> launchers = new Dictionary<string, ILauncher>();

        static LauncherRegistry()
        {
            Load();
        }

        public static ILauncher GetLauncher(string editorPath)
        {
            _ = launchers.TryGetValue(editorPath, out ILauncher launcher);
            return launcher;
        }

        public static void Load()
        {
            launchers.Clear();
            LoadErrors = string.Empty;
            List<CodeEditor.Installation> installations = new List<CodeEditor.Installation>();
            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && typeof(ILauncher).IsAssignableFrom(t)))
            {
                ILauncher instance = (ILauncher)System.Activator.CreateInstance(type);

                if (instance.Installations == null)
                {
                    continue;
                }

                foreach (CodeEditor.Installation installation in instance.Installations)
                {
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


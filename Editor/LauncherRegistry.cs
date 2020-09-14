using UnityEditor;
using System.Reflection;
using System.Linq;
using Unity.CodeEditor;
using System.Collections.Generic;

namespace EasyEditor
{
    [InitializeOnLoad]
    internal static class LauncherRegistry
    {
        public static CodeEditor.Installation[] Installations { get; set; }
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

        public static ILauncher[] GetLaunchers()
        {
            return launchers.Values.ToArray();
        }

        public static void Load()
        {
            launchers.Clear();
            List<CodeEditor.Installation> installations = new List<CodeEditor.Installation>();
            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "EasyEditor.Launchers"))
            {
                if (type == null)
                {
                    continue;
                }

                if (type.IsAssignableFrom(typeof(ILauncher)))
                {
                    continue;
                }

                LauncherAttribute attribute = type.GetCustomAttribute<LauncherAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.InstallationNames == null)
                {
                    continue;
                }

                ILauncher instance = (ILauncher)System.Activator.CreateInstance(type);
                foreach (string name in attribute.InstallationNames)
                {
                    foreach (string editorPath in instance.InstallationPaths)
                    {
                        CodeEditor.Installation installation = new CodeEditor.Installation
                        {
                            Name = name,
                            Path = editorPath
                        };
                        installations.Add(installation);
                        launchers.Add(editorPath, instance);
                    }
                }
            }

            Installations = installations.ToArray();
        }
    }
}


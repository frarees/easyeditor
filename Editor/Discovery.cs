namespace EasyEditor
{
    using System.Collections.Generic;
    using System.IO;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    internal class Discovery
    {
        public string name = default;
        public string executable = default;
        public string[] paths = default;
        public string defaultArguments = default;

        public bool inheritsEnvironmentVariables = false;
        public bool requiresNativeOpen = false;

        [TextArea] public string notes = default;

        private List<string> allPaths = new List<string>();

        public bool AutoGenerate
        {
            get => EditorPrefs.GetBool($"EasyEditor.{name}.AutoGenerate", true);
            set => EditorPrefs.SetBool($"EasyEditor.{name}.AutoGenerate", value);
        }

        public bool ExportFrameworkPathOverride
        {
            get => EditorPrefs.GetBool($"EasyEditor.{name}.ExportFrameworkPathOverride", !requiresNativeOpen && inheritsEnvironmentVariables);
            set => EditorPrefs.SetBool($"EasyEditor.{name}.ExportFrameworkPathOverride", value && !requiresNativeOpen && inheritsEnvironmentVariables);
        }

#if !UNITY_2020_2_OR_NEWER
        public bool MatchCompilerVersion
        {
            get => EditorPrefs.GetBool($"EasyEditor.{name}.MatchCompilerVersion", true);
            set => EditorPrefs.SetBool($"EasyEditor.{name}.MatchCompilerVersion", value);
        }
#endif

        public string Arguments
        {
            get => EditorPrefs.GetString($"EasyEditor.{name}.Arguments", defaultArguments);
            set => EditorPrefs.SetString($"EasyEditor.{name}.Arguments", value);
        }

        public bool TryGetFirstValidInstallation(out CodeEditor.Installation installation)
        {
            allPaths.Clear();

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
#if UNITY_EDITOR_WIN
                path = path.Replace("$(ProgramFiles)", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles));
                path = path.Replace("$(ProgramFiles(x86))", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86));
#endif
                allPaths.Add(path);
            }

            string pathEnv = System.Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                allPaths.AddRange(pathEnv.Split(':'));
            }

            foreach (string path in allPaths)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                string editorPath = Path.Combine(path, executable);
                if (!File.Exists(editorPath))
                {
                    continue;
                }

                installation = new CodeEditor.Installation()
                {
                    Name = name,
                    Path = editorPath,
                };
                return true;
            }

            installation = default;
            return false;
        }
    }
}


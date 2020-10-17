namespace EasyEditor
{
    using System.Collections.Generic;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DiscoveryRegistry.asset", menuName = "Easy Editor/Discovery Registry", order = 150)]
    internal class Registry : ScriptableObject
    {
        public static Registry Instance { get; private set; }

        public List<Discovery> discoveries = new List<Discovery>();

        private List<CodeEditor.Installation> loadedInstallations = new List<CodeEditor.Installation>();
        private List<Discovery> loadedDiscoveries = new List<Discovery>();

        public static bool LoadInstance()
        {
#if UNITY_EDITOR_OSX
            Instance = AssetDatabase.LoadAssetAtPath<Registry>("Packages/com.frarees.easyeditor/Editor/Registries/macOS.asset");
#elif UNITY_EDITOR_WIN
            Instance = AssetDatabase.LoadAssetAtPath<Registry>("Packages/com.frarees.easyeditor/Editor/Registries/Windows.asset");
#else
            Instance = AssetDatabase.LoadAssetAtPath<Registry>("Packages/com.frarees.easyeditor/Editor/Registries/Linux.asset");
#endif
            if (!Instance)
            {
                return false;
            }

            Instance.Reload();
            return true;
        }

        public CodeEditor.Installation[] GetInstallations()
        {
            return loadedInstallations.ToArray();
        }

        public void Reload()
        {
            loadedInstallations.Clear();
            loadedDiscoveries.Clear();

            foreach (Discovery discovery in discoveries)
            {
                if (discovery.TryGetFirstValidInstallation(out CodeEditor.Installation installation))
                {
                    loadedInstallations.Add(installation);
                    loadedDiscoveries.Add(discovery);
                }
            }
        }

        public bool TryGetDiscoveryFromEditorPath(string editorPath, out Discovery discovery)
        {
            for (int i = 0; i < loadedInstallations.Count; i++)
            {
                if (loadedInstallations[i].Path == editorPath)
                {
                    discovery = loadedDiscoveries[i];
                    return true;
                }
            }

            discovery = default;
            return false;
        }
    }
}


namespace EasyEditor
{
    using System.IO;
    using System.Xml;
    using UnityEditor;
#if !UNITY_2020_2_OR_NEWER
    using Unity.CodeEditor;
#endif

    internal class Postprocessor : AssetPostprocessor
    {
        private static void OnGeneratedCSProjectFiles()
        {
#if !UNITY_2020_2_OR_NEWER
            if (!Preferences.IsActive)
            {
                return;
            }

            if (Registry.Instance.TryGetDiscoveryFromEditorPath(CodeEditor.CurrentEditorInstallation, out Discovery discovery))
            {
                if (discovery.MatchCompilerVersion)
                {
                    WriteLangVersions();
                }
            }
#endif
        }

#if !UNITY_2020_2_OR_NEWER
        private static void WriteLangVersions()
        {
            foreach (string path in Directory.GetFiles(Preferences.projectPath, "*.csproj"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
                ns.AddNamespace("msbld", "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlNode node = document.SelectSingleNode("//msbld:LangVersion", ns);
                node.LastChild.InnerText = Preferences.GetLangVersion();
                document.Save(path);
            }
        }
#endif
    }
}


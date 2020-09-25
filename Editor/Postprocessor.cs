namespace EasyEditor
{
    using System.IO;
    using System.Xml;
    using UnityEditor;

    internal class Postprocessor : AssetPostprocessor
    {
        private static void OnGeneratedCSProjectFiles()
        {
            if (!Preferences.IsActive)
            {
                return;
            }

            if (Preferences.MatchCompilerVersion)
            {
                WriteLangVersions();
            }
        }

        private static void WriteLangVersions()
        {
            foreach (string path in Directory.GetFiles(Preferences.ProjectPath, "*.csproj"))
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
    }
}


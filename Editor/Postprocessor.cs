﻿namespace EasyEditor
{
    using UnityEditor;
    using System.IO;
    using System.Xml;

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
                node.LastChild.InnerText = GetLangVersion();
                document.Save(path);
            }
        }

        private static string GetLangVersion()
        {
#if UNITY_2020_2_OR_NEWER
            return "8.0";
#else
            return "7.3";
#endif
        }
    }
}

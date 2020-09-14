namespace EasyEditor
{
    internal class LauncherAttribute : System.Attribute
    {
        public string[] InstallationNames { get; }
        public LauncherAttribute(params string[] installationNames) { InstallationNames = installationNames; }
    }
}


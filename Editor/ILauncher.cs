namespace EasyEditor
{
    internal interface ILauncher
    {
        string[] InstallationPaths { get; }

        bool Launch(string editorPath, LaunchDescriptor descriptor);
        void SyncAll();
        void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles);

        bool MatchesExecutable(string editorPath);
    }
}


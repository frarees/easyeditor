namespace EasyEditor
{
    using Unity.CodeEditor;

    internal abstract class Launcher
    {
        public abstract CodeEditor.Installation[] Installations { get; }

        public abstract bool Launch(string editorPath, LaunchDescriptor descriptor);
        public abstract void SyncAll();
        public abstract void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles);
        public abstract bool MatchesExecutable(string editorPath);

        public abstract void OnGUI();
    }
}


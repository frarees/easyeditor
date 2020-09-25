namespace EasyEditor
{
    internal readonly struct LaunchDescriptor
    {
        public string FilePath { get; }
        public int Line { get; }
        public int Column { get; }
        public string ProjectPath { get; }
        public string ProjectName { get; }

        public LaunchDescriptor(string filePath, int line, int column, string projectPath, string projectName)
        {
            FilePath = filePath;
            Line = line;
            Column = column;
            ProjectPath = projectPath;
            ProjectName = projectName;
        }
    }
}


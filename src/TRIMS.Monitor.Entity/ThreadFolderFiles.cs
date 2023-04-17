namespace TRIMS.Monitor.Entity
{
    public class ThreadFolderFiles
    {
        public ThreadFolderFiles(string threadName, FolderType folder, FileDetail[]? files)
        {
            ThreadName = threadName;
            Files = files;
            Folder = folder;
        }

        public string ThreadName { get; set; } = string.Empty;
        public FolderType Folder { get; set; } = FolderType.SourceFolder;
        public string FolderPath { get; set; } = string.Empty;
        public FileDetail[]? Files { get; set; }
    }
}

namespace CleanSweep.Application.Vsix
{
    using System.IO;

    public class WindowsFileSystem : IFileSystemAdapter
    {
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string GetDirectoryName(string fileName)
        {
            return Path.GetDirectoryName(fileName);
        }

        public string ReadAllFileText(string fileNAme)
        {
            return File.ReadAllText(fileNAme);
        }
    }
}
namespace CleanSweep.Adapter.Contract
{
    public interface IFileSystemAdapter
    {
        string ReadAllFileText(string solutionFileName);
        string GetDirectoryName(string solutionFileName);
        bool DirectoryExists(string path);
        void DeleteDirectory(string path);
    }
}
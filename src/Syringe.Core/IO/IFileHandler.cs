using System.Collections.Generic;

namespace Syringe.Core.IO
{
    public interface IFileHandler
    {
        string GetFileFullPath(string branchName, string fileName);
        string ReadAllText(string path);
        bool WriteAllText(string path, string contents);
        IEnumerable<string> GetFileNames();
        bool FileExists(string filePath);
        string CreateFileFullPath(string branchName, string fileName);
        string CreateFilename(string filename);
        bool DeleteFile(string path);
    }
}
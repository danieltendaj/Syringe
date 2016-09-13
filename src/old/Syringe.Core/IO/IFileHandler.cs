using System.Collections.Generic;

namespace Syringe.Core.IO
{
    public interface IFileHandler
    {
        string GetFileFullPath(string fileName);
        string ReadAllText(string path);
        bool WriteAllText(string path, string contents);
        IEnumerable<string> GetFileNames();
        bool FileExists(string filePath);
        string CreateFileFullPath(string fileName);
        string GetFilenameWithExtension(string filename);
        bool DeleteFile(string path);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;

namespace Syringe.Core.IO
{
    public class FileHandler : IFileHandler
    {
        private readonly IConfiguration _configuration;

        public FileHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFileFullPath(string branchName, string fileName)
        {
	        string fullPath = CreateFileFullPath(branchName, fileName);

			if (!File.Exists(fullPath))
                throw new FileNotFoundException("The test file path cannot be found", fileName);

            return fullPath;
        }

        public string CreateFileFullPath(string branchName, string fileName)
        {
			string fullPath = "";

			if (!string.IsNullOrEmpty(branchName))
			{
				fullPath = Path.Combine(_configuration.TestFilesBaseDirectory, branchName, fileName);
			}
			else
			{
				fullPath = Path.Combine(_configuration.TestFilesBaseDirectory, fileName);
			}

	        return fullPath;
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public bool WriteAllText(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.Message);
            }

            return false;
        }

        public IEnumerable<string> GetFileNames()
        {
            foreach (string file in Directory.EnumerateFiles(_configuration.TestFilesBaseDirectory, "*.xml"))
            {
                var fileInfo = new FileInfo(file);
                yield return fileInfo.Name;
            }
        }

        public string CreateFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!filename.EndsWith(".xml"))
            {
                filename = string.Concat(filename, ".xml");
            }

            return filename;
        }

        public bool DeleteFile(string path)
        {
            try
            {
				FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs,RecycleOption.SendToRecycleBin);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.Message);
            }

            return false;
        }
    }
}

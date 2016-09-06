using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;
using SearchOption = System.IO.SearchOption;

namespace Syringe.Core.IO
{
    public class FileHandler : IFileHandler
    {
        private readonly IConfiguration _configuration;
        private const string FileExtension = "json";

        public FileHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFileFullPath(string fileName)
        {
	        string fullPath = CreateFileFullPath(fileName);

			if (!File.Exists(fullPath))
                throw new FileNotFoundException("The test file path cannot be found", fileName);

            return fullPath;
        }

        public string CreateFileFullPath(string fileName)
        {
			return Path.Combine(_configuration.TestFilesBaseDirectory, fileName);
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
                Log.Error(exception, $"path={path}\ncontents={contents}\n\n{exception.Message}");
            }

            return false;
        }

        public IEnumerable<string> GetFileNames()
        {
            foreach (string file in Directory.EnumerateFiles(_configuration.TestFilesBaseDirectory, "*." + FileExtension, SearchOption.AllDirectories))
            {
                yield return file.Substring(_configuration.TestFilesBaseDirectory.Length);
            }
        }

        public string GetFilenameWithExtension(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!filename.EndsWith("." + FileExtension))
            {
                filename = string.Concat(filename, "." + FileExtension);
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

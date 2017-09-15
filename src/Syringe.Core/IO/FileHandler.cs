using System;
using System.Collections.Generic;
using System.IO;
using Syringe.Core.Configuration;
using SearchOption = System.IO.SearchOption;

namespace Syringe.Core.IO
{
	public class FileHandler : IFileHandler
	{
		private readonly Settings _settings;
		private const string FileExtension = "json";

		public FileHandler(Settings settings)
		{
			_settings = settings;
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
			return Path.Combine(_settings.TestFilesBaseDirectory, fileName);
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
			File.WriteAllText(path, contents);
			return true;
		}

		public IEnumerable<string> GetFileNames()
		{
			string baseDirectory = _settings.TestFilesBaseDirectory;

			foreach (string file in Directory.EnumerateFiles(baseDirectory, "*." + FileExtension, SearchOption.AllDirectories))
			{
				string relativeFileName = file.Substring(baseDirectory.Length);
				relativeFileName = relativeFileName.TrimStart('/', '\\');

				yield return relativeFileName;
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
			File.Delete(path);
			return true;
		}
	}
}
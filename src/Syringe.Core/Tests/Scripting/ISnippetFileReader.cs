using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Tests.Scripting
{
	public interface ISnippetFileReader
	{
		string ReadFile(string path);
		IEnumerable<string> GetSnippetFilenames();
	}

	public class SnippetFileReader : ISnippetFileReader
	{
		private IConfiguration _configuration;

		public SnippetFileReader(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEnumerable<string> GetSnippetFilenames()
		{
			if (!File.Exists(_configuration.ScriptSnippetDirectory))
				return new string[] {};

			return Directory.EnumerateFiles(_configuration.ScriptSnippetDirectory, "*.snippet");
		}

		public string ReadFile(string path)
		{
			string fullPath = Path.Combine(_configuration.ScriptSnippetDirectory, path);
			return File.ReadAllText(fullPath);
		}
	}
}

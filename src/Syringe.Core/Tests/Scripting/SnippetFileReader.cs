using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.Configuration;

namespace Syringe.Core.Tests.Scripting
{
	public class SnippetFileReader : ISnippetFileReader
	{
		private readonly IConfiguration _configuration;

		public SnippetFileReader(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEnumerable<string> GetSnippetFilenames(ScriptSnippetType snippetType)
		{
			if (!Directory.Exists(_configuration.ScriptSnippetDirectory))
				return new string[] {};

			string fullPath = Path.Combine(_configuration.ScriptSnippetDirectory,
											snippetType.ToString().ToLower());

			return Directory.EnumerateFiles(fullPath, "*.snippet")
					.Select(x => new FileInfo(x).Name);
		}

		public string ReadFile(string path)
		{
			string fullPath = Path.Combine(_configuration.ScriptSnippetDirectory, path);
			return File.ReadAllText(fullPath);
		}
	}
}
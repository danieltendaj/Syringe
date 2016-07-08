using Syringe.Core.Configuration;
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
	}

	public class SnippetFileReader : ISnippetFileReader
	{
		private IConfiguration _configuration;

		public SnippetFileReader(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string ReadFile(string path)
		{
			string fullPath = Path.Combine(_configuration.ScriptSnippetDirectory, path);
			return File.ReadAllText(fullPath);
		}
	}
}

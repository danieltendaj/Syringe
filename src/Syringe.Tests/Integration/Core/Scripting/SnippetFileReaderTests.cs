using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Syringe.Tests.Integration.Core.Scripting
{
	public class SnippetFileReaderTests
	{
		private string _snippetDirectory;

		[SetUp]
		public void SetUp()
		{
			_snippetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "ScriptSnippets");
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());

			if (Directory.Exists(typeDirectory))
			{
				Directory.Delete(typeDirectory, true);
			}

			Directory.CreateDirectory(typeDirectory);
		}

		[Test]
		public void should_read_text_file()
		{
			// Arrange
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());

			string filename1 = Path.Combine(typeDirectory, "snippet1.snippet");
			File.WriteAllText(filename1, "snippet 1");

			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = _snippetDirectory;

			var snippetReader = new SnippetFileReader(config);

			// Act
			string snippetText = snippetReader.ReadFile(filename1);

			// Assert
			Assert.That(snippetText, Is.EqualTo("snippet 1"));
		}


		[Test]
		public void should_return_empty_list_when_snippet_directory_does_not_exist()
		{
			// Arrange
			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = "doesnt-exist";
			var snippetReader = new SnippetFileReader(config);

			// Act
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// Assert
			Assert.That(files.Count(), Is.EqualTo(0));
		}

		[Test]
		public void should_return_empty_list_when_snippet_sub_directory_does_not_exist()
		{
			// Arrange
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());
			try
			{
				Directory.Delete(typeDirectory);
			}
			catch (IOException)
			{
			}

			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = _snippetDirectory;
			var snippetReader = new SnippetFileReader(config);

			// Act
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// Assert
			Assert.That(files.Count(), Is.EqualTo(0));
		}

		[Test]
		public void should_get_snippet_filenames_from_directory()
		{
			// Arrange
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());

			string filename1 = Path.Combine(typeDirectory, "snippet1.snippet");
			string filename2 = Path.Combine(typeDirectory, "snippet2.snippet");

			File.WriteAllText(filename1, "snippet 1");
			File.WriteAllText(filename2, "snippet 2");

			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = _snippetDirectory;
			var snippetReader = new SnippetFileReader(config);

			// Act
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// Assert
			Assert.That(files.Count(), Is.EqualTo(2));
			Assert.That(files, Contains.Item("snippet1.snippet"));
			Assert.That(files, Contains.Item("snippet2.snippet"));
		}
	}
}

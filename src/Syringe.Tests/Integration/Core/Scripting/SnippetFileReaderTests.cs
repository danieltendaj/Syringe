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
			// given
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());

			string filename1 = Path.Combine(typeDirectory, "snippet1.snippet");
			File.WriteAllText(filename1, "snippet 1");

			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = _snippetDirectory;

			var snippetReader = new SnippetFileReader(config);

			// when
			string snippetText = snippetReader.ReadFile(filename1);

			// then
			Assert.That(snippetText, Is.EqualTo("snippet 1"));
		}


		[Test]
		public void should_return_empty_list_when_snippet_directory_does_not_exist()
		{
			// given
			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = "doesnt-exist";
			var snippetReader = new SnippetFileReader(config);

			// when
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// then
			Assert.That(files.Count(), Is.EqualTo(0));
		}

		[Test]
		public void should_return_empty_list_when_snippet_sub_directory_does_not_exist()
		{
			// given
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

			// when
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// then
			Assert.That(files.Count(), Is.EqualTo(0));
		}

		[Test]
		public void should_get_snippet_filenames_from_directory()
		{
			// given
			string typeDirectory = Path.Combine(_snippetDirectory, ScriptSnippetType.BeforeExecute.ToString().ToLower());

			string filename1 = Path.Combine(typeDirectory, "snippet1.snippet");
			string filename2 = Path.Combine(typeDirectory, "snippet2.snippet");

			File.WriteAllText(filename1, "snippet 1");
			File.WriteAllText(filename2, "snippet 2");

			var config = new JsonConfiguration();
			config.ScriptSnippetDirectory = _snippetDirectory;
			var snippetReader = new SnippetFileReader(config);

			// when
			IEnumerable<string> files = snippetReader.GetSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// then
			Assert.That(files.Count(), Is.EqualTo(2));
			Assert.That(files, Contains.Item("snippet1.snippet"));
			Assert.That(files, Contains.Item("snippet2.snippet"));
		}
	}
}

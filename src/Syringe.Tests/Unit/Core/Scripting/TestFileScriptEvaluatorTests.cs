﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Scripting;

namespace Syringe.Tests.Unit.Core.Scripting
{
	public class SnippetFileReaderMock : ISnippetFileReader
	{
		private string _content;

		public SnippetFileReaderMock(string content)
		{
			_content = content;
		}

		public string ReadFile(string path)
		{
			return _content;
		}

		public IEnumerable<string> GetSnippetFilenames()
		{
			return new string[] {"file1", "file2"};
		}
	}

	public class TestFileScriptEvaluatorTests
	{
		[Test]
		public void Should_throw_CodeEvaluationException_with_test_and_compilation_information_in_exception_message()
		{
			// given
			string code = "this won't compile";
			var snippetReader = new SnippetFileReaderMock(code);
			var evaluator = new TestFileScriptEvaluator(new JsonConfiguration(), snippetReader);
			var test = new Test()
			{
                ScriptSnippets = new ScriptSnippets()
                {
                    BeforeExecuteFilename = "filepath-isnt-used.snippet"  
                },
				Description = "My test"
			};

			// when + then
			try
			{
				evaluator.EvaluateBeforeExecute(test, new RestRequest());
				Assert.Fail("Expected a CodeEvaluationException");
			}
			catch (CodeEvaluationException ex)
			{
				Assert.That(ex.Message, Contains.Substring("An exception occurred evaluating the before script for test 'My test'"));
				Assert.That(ex.Message, Contains.Substring("error CS1002: ; expected"));
				Assert.That(ex.InnerException, Is.Not.Null);
				Assert.That(ex.InnerException, Is.TypeOf<CompilationErrorException>());
			}
		}

		[Test]
		public void EvaluateBeforeExecute_should_add_required_references()
		{
			// given
			var snippetReader = new SnippetFileReaderMock("IRestRequest request = new RestRequest();");
			var evaluator = new TestFileScriptEvaluator(new JsonConfiguration(), snippetReader);
			var test = new Test();
			test.ScriptSnippets.BeforeExecuteFilename = "path-doesnt-matter.snippet";

			// when + then
			evaluator.EvaluateBeforeExecute(test, new RestRequest());
		}

		[Test]
		public void EvaluateBeforeExecute_should_set_globals()
		{
			// given
			string code = "Test.Description = \"it worked\";" +
						  "Request.Method = Method.PUT;";

			var snippetReader = new SnippetFileReaderMock(code);
			var evaluator = new TestFileScriptEvaluator(new JsonConfiguration(), snippetReader);
			var test = new Test();
			test.ScriptSnippets.BeforeExecuteFilename = "filename-doesnt-matter.snippet";

			// when
			evaluator.EvaluateBeforeExecute(test, new RestRequest());

			// then
			Assert.That(evaluator.RequestGlobals.Test.Description, Is.EqualTo("it worked"));
			Assert.That(evaluator.RequestGlobals.Request.Method, Is.EqualTo(Method.PUT));
		}
	}
}
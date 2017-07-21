using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Syringe.Core.Tests.Scripting
{
	public class TestFileScriptEvaluator
	{
		private readonly ISnippetFileReader _snippetReader;
		private readonly IConfiguration _configuration;

		public RequestGlobals RequestGlobals { get; set; }

		public TestFileScriptEvaluator(IConfiguration configuration, ISnippetFileReader snippetReader)
		{
			RequestGlobals = new RequestGlobals();
			RequestGlobals.Configuration = configuration;

			_configuration = configuration;
			_snippetReader = snippetReader;
		}

		public bool EvaluateBeforeExecute(Test test, HttpRequestMessage request)
		{
			if (string.IsNullOrEmpty(test.ScriptSnippets.BeforeExecuteFilename))
			{
				return false;
			}

			string scriptContent = "";

			try
			{
				string typeName = ScriptSnippetType.BeforeExecute.ToString().ToLower();
				string path = Path.Combine(_configuration.ScriptSnippetDirectory, typeName, test.ScriptSnippets.BeforeExecuteFilename);
				scriptContent = _snippetReader.ReadFile(path);
			}
			catch (IOException ex)
			{
				string message = "An exception occurred loading the snippet {0} for test '{1}': \n{2}";
				throw new CodeEvaluationException(ex, message, test.ScriptSnippets.BeforeExecuteFilename, test.Description, ex.ToString());
			}

			RequestGlobals.Test = test;
			RequestGlobals.Request = request;

			ScriptOptions scriptOptions = ScriptOptions.Default
				.WithReferences(typeof(HttpRequestMessage).GetTypeInfo().Assembly)
				.AddImports(new[] { "RestSharp" });

			try
			{
				CSharpScript.EvaluateAsync(scriptContent, options: scriptOptions, globals: RequestGlobals).Wait();
				return true;
			}
			catch (CompilationErrorException ex)
			{
				string message = "An exception occurred evaluating the before script for test '{0}': \n{1}";
				throw new CodeEvaluationException(ex, message, test.Description, ex.ToString());
			}
		}
	}
}
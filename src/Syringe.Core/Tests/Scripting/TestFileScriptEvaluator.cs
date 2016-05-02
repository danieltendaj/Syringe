using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Tests.Scripting
{
	public class TestFileScriptEvaluator
	{
		public RequestGlobals RequestGlobals { get; set; }

		public TestFileScriptEvaluator(IConfiguration configuration)
		{
			RequestGlobals = new RequestGlobals();
			RequestGlobals.Configuration = configuration;
		}

		public void EvaluateBeforeExecute(Test test, IRestRequest request)
		{
			RequestGlobals.Test = test;
			RequestGlobals.Request = request;

			ScriptOptions scriptOptions = ScriptOptions.Default
				.WithReferences(typeof (IRestRequest).Assembly)
				.AddImports(new[] {"RestSharp"});

			try
			{
				CSharpScript.EvaluateAsync(test.BeforeExecuteScript, options: scriptOptions, globals: RequestGlobals).Wait();
			}
			catch (CompilationErrorException ex)
			{
				string message = "An exception occurred evaluating the before script for test '{0}': \n{1}";
				throw new CodeEvaluationException(ex, message, test.Description, ex.ToString());
			}
		}
	}
}

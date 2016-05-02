using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Exceptions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Scripting;

namespace Syringe.Tests.Unit.Core.Scripting
{
	public class TestFileScriptEvaluatorTests
	{
		[Test]
		public void Should_throw_CodeEvaluationException_with_test_and_compilation_information_in_exception_message()
		{
			// given
			var evaluator = new TestFileScriptEvaluator();
			string code = "this won't compile";
			var test = new Test() { Description = "My test" };

			// when + then
			try
			{
				evaluator.EvaluateBeforeExecute(code, test, new RestRequest());
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
			var evaluator = new TestFileScriptEvaluator();

			// when + then
			evaluator.EvaluateBeforeExecute("IRestRequest request = new RestRequest();", new Test(), new RestRequest());
		}

		[Test]
		public void EvaluateBeforeExecute_should_set_globals()
		{
			// given
			var evaluator = new TestFileScriptEvaluator();
			string code = "Test.Description = \"it worked\";" +
						  "Request.Method = Method.PUT;";

			// when
			evaluator.EvaluateBeforeExecute(code, new Test(), new RestRequest());

			// then
			Assert.That(evaluator.RequestGlobals.Test.Description, Is.EqualTo("it worked"));
			Assert.That(evaluator.RequestGlobals.Request.Method, Is.EqualTo(Method.PUT));
		}
	}
}

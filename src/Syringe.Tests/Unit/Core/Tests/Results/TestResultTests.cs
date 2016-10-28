using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Core.Tests.Results
{
	public class TestResultTests
	{
		[Test]
		public void VerifyPositivesSuccess_should_return_false_when_all_positive_results_are_false()
		{
			// given
			var testResult = new TestResult();
			testResult.AssertionResults.Add(new Assertion("desc", "regex", AssertionType.Positive, AssertionMethod.Regex) { Success = false });

			// when
			bool actualResult = testResult.AssertionsSuccess;

			// then
			Assert.That(actualResult, Is.False);
		}

		[Test]
		public void VerifyNegativeSuccess_should_return_false_when_all_positive_results_are_false()
		{
			// given
			var testResult = new TestResult();
			testResult.AssertionResults.Add(new Assertion("desc", "regex", AssertionType.Negative, AssertionMethod.Regex) { Success = false });

			// when
			bool actualResult = testResult.AssertionsSuccess;

			// then
			Assert.That(actualResult, Is.False);
		}

		[Test]
		public void VerifyPositivesSuccess_and_VerifyNegativeSuccess_should_return_true_when_positiveresults_is_null()
		{
			// given
			var testResult = new TestResult();

			// when + then
			Assert.That(testResult.AssertionsSuccess, Is.True);
			Assert.That(testResult.AssertionsSuccess, Is.True);
		}
	}
}

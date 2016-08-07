using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Core.Tests.Results
{
	public class TestResultTests
	{
		[Test]
		[TestCase(true, true, true, true)]
		[TestCase(false, false, true, true)]
		[TestCase(false, false, true, true)]
		[TestCase(false, true, false, true)]
		[TestCase(false, true, true, false)]
		public void Success_should_return_result_based_on_success_codes(bool expectedResult, bool responseCodeSuccess, bool positiveSuccess, bool negativeSuccess)
		{
			// given
			var testResult = new TestResult();
			testResult.ResponseCodeSuccess = responseCodeSuccess;
			testResult.AssertionResults.Add(new Assertion("desc", "regex", AssertionType.Positive, AssertionMethod.Regex) { Success = positiveSuccess });
			testResult.AssertionResults.Add(new Assertion("desc", "regex", AssertionType.Negative, AssertionMethod.Regex) { Success = negativeSuccess });

			// when
			bool actualResult = testResult.Success;

			// then
			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

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

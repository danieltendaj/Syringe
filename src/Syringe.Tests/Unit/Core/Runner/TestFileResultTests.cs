using NUnit.Framework;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Core.Runner
{
	public class TestFileResultTests
	{
		[Test]
		public void TotalTestsPassed_should_count_passed_tests()
		{
			// given
			var builder = new TestFileResultsBuilder()
								.New().WithSuccess().Add()
								.New().WithSuccess().Add()
								.New().WithSuccess().Add()
								.New().WithFail().Add()
								.New().WithFail().Add();

			var testFileResult = new TestFileResult();
			testFileResult.TestResults = builder.GetCollection();

			// when + then
			Assert.That(testFileResult.TotalTestsPassed, Is.EqualTo(3));
        }

        [Test]
        public void TotalTestsFailed_should_count_passed_tests()
        {
            // given
            var builder = new TestFileResultsBuilder()
                                .New().WithSuccess().Add()
                                .New().WithSuccess().Add()
                                .New().WithFail().Add()
                                .New().WithFail().Add()
                                .New().WithFail().Add();

            var testFileResult = new TestFileResult();
            testFileResult.TestResults = builder.GetCollection();

            // when + then
            Assert.That(testFileResult.TotalTestsFailed, Is.EqualTo(3));
        }

        [Test]
        public void TotalTestsFailed_should_count_skipped_tests()
        {
            // given
            var builder = new TestFileResultsBuilder()
                                .New().WithSkipped().Add()
                                .New().WithSkipped().Add()
                                .New().WithFail().Add()
                                .New().WithFail().Add()
                                .New().WithFail().Add();

            var testFileResult = new TestFileResult();
            testFileResult.TestResults = builder.GetCollection();

            // when + then
            Assert.That(testFileResult.TotalTestsSkipped, Is.EqualTo(2));
        }

        [Test]
		public void TotalAssertionsPassed_should_count_passed_tests()
		{
			// given
			var builder = new TestFileResultsBuilder()
								.New().AddPositiveVerify().Add()
								.New().AddNegativeVerify().Add()
								.New().AddPositiveVerify(false).Add()
								.New().AddNegativeVerify(false).Add();

			var testFileResult = new TestFileResult();
			testFileResult.TestResults = builder.GetCollection();

			// when + then
			Assert.That(testFileResult.TotalAssertionsPassed, Is.EqualTo(2));
		}

		[Test]
		public void TotalAssertionsFailed_should_count_failed_tests()
		{
			// given
			var builder = new TestFileResultsBuilder()
								.New().AddPositiveVerify().Add()
								.New().AddPositiveVerify(false).Add()
								.New().AddNegativeVerify(false).Add();

			var testFileResult = new TestFileResult();
			testFileResult.TestResults = builder.GetCollection();

			// when + then
			Assert.That(testFileResult.TotalAssertionsFailed, Is.EqualTo(2));
		}
	}
}

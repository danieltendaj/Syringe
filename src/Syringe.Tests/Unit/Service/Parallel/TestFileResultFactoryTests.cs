using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Syringe.Core.Tests.Results;
using Syringe.Service.Parallel;

namespace Syringe.Tests.Unit.Service.Parallel
{
    [TestFixture]
    public class TestFileResultFactoryTests
    {
        [Test]
        public void should_return_error_if_timed_out()
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var factory = new TestFileResultFactory();

            // when
            var result = factory.Create(null, true, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Completed, Is.False);
            Assert.That(result.TimeTaken, Is.EqualTo(timeTaken));
            Assert.That(result.ErrorMessage, Is.EqualTo("The runner timed out."));
        }

        [Test]
        public void should_return_error_if_errors_exist()
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                Errors = "WOOPEEE"
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Completed, Is.False);
            Assert.That(result.TimeTaken, Is.EqualTo(timeTaken));
            Assert.That(result.ErrorMessage, Is.EqualTo(runnerInfo.Errors));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void should_return_result_result_id_if_it_exists(bool idExists)
        {
            // given
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                TestFileResults = idExists ? new TestFileResult {Id = Guid.NewGuid()} : null 
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo(string.Empty));

            if (idExists)
            {
                Assert.That(result.ResultId, Is.EqualTo(runnerInfo.TestFileResults.Id));
            }
            else
            {
                Assert.That(result.ResultId, Is.Null);
            }
        }
    }
}
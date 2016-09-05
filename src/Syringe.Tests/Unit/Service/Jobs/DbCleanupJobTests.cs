using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Service.Jobs;

namespace Syringe.Tests.Unit.Service.Jobs
{
    [TestFixture]
    public class DbCleanupJobTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ITestFileResultRepository> _repositoryMock;
        private DbCleanupJob _job;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _repositoryMock = new Mock<ITestFileResultRepository>();
            _job = new DbCleanupJob(_configurationMock.Object, _repositoryMock.Object);
        }

        [Test]
        public void should_clear_results_before_now()
        {
            // given
            const int expectedDaysOfRetention = 66;
            _configurationMock
                .Setup(x => x.DaysOfDataRetention)
                .Returns(expectedDaysOfRetention);

            // when
            _job.Cleanup();

            // then
            _repositoryMock
                .Verify(x => x.DeleteBeforeDate(DateTime.Today.AddDays(-expectedDaysOfRetention)), Times.Once);
        }
    }
}
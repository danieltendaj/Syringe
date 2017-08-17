using System;
using System.Threading;
using Moq;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories;
using Syringe.Service.Jobs;
using Syringe.Tests.NUnitToXUnit;
using Xunit;

namespace Syringe.Tests.Unit.Service.Jobs
{
	public class DbCleanupJobTests
	{
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<ITestFileResultRepository> _repositoryMock;
		private int _callbackCount;
		private DbCleanupJob _job;

		public DbCleanupJobTests()
		{
			_callbackCount = 0;
			_configurationMock = new Mock<IConfiguration>();
			_repositoryMock = new Mock<ITestFileResultRepository>();
			_job = new DbCleanupJob(_configurationMock.Object, _repositoryMock.Object);
		}

		[Fact]
		public void should_clear_results_before_now()
		{
			// given
			const int expectedDaysOfRetention = 66;
			_configurationMock
				.Setup(x => x.DaysOfDataRetention)
				.Returns(expectedDaysOfRetention);

			// when
			_job.Cleanup(null);

			// then
			_repositoryMock
				.Verify(x => x.DeleteBeforeDate(DateTime.Today.AddDays(-expectedDaysOfRetention)), Times.Once);
		}

		[Fact]
		public void should_execute_given_callback_via_timer_and_then_stop()
		{
			// given
			_configurationMock
				.Setup(x => x.CleanupSchedule)
				.Returns(new TimeSpan(0, 0, 0, 0, 10)); // 10 milli

			// when
			_job.Start(DummyCallback);

			// then
			Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50)); // 50 milli
			Assert.InRange(_callbackCount, 3, Int32.MaxValue);

			_job.Stop();
			int localCallbackStore = _callbackCount;
			Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50)); // 50 milli
			NUnitAssert.That(_callbackCount, Is.EqualTo(localCallbackStore));
		}

		[Fact]
		public void start_should_clear_data()
		{
			// given

			// when
			_job.Start();
			Thread.Sleep(new TimeSpan(0, 0, 0, 0, 50)); // 50 milli

			// then
			_repositoryMock
				.Verify(x => x.DeleteBeforeDate(It.IsAny<DateTime>()), Times.AtLeastOnce);

			_job.Stop();
		}

		private void DummyCallback(object guff)
		{
			_callbackCount++;
		}
	}
}
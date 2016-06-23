using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Syringe.Core.Configuration;
using Syringe.Core.MongoDB;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Tests.Integration.Core.Repository.MongoDB
{
	public class TestFileResultRepositoryTests
	{
		private TestFileResultRepository GetTestFileResultRepository()
		{
			return new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = "Syringe-Tests"});	
		}

		[SetUp]
		public void SetUp()
		{
			GetTestFileResultRepository().Wipe();
		}

		[Test]
		public async Task Add_should_save_session()
		{
			// Arrange
			var fixture = new Fixture();
			var session = fixture.Create<TestFileResult>();

			TestFileResultRepository repository = GetTestFileResultRepository();

			// Act
			await repository.AddAsync(session);

            // Assert
            TestFileResultSummaryCollection summaries = await repository.GetSummaries(It.IsAny<DateTime>());
			Assert.That(summaries.TotalFileResults, Is.EqualTo(1));
		}

		[Test]
		public async Task Delete_should_remove_the_session()
		{
			// Arrange
			var fixture = new Fixture();
			var session = fixture.Create<TestFileResult>();

			TestFileResultRepository repository = GetTestFileResultRepository();
			await repository.AddAsync(session);

			// Act
			await repository.DeleteAsync(session.Id);

            // Assert
            TestFileResultSummaryCollection summaries = await repository.GetSummaries(It.IsAny<DateTime>());
			Assert.That(summaries.PagedResults.Count(), Is.EqualTo(0));
		}

		[Test]
		public async Task GetById_should_return_session()
		{
			// Arrange
			var fixture = new Fixture();
			var expectedSession = fixture.Create<TestFileResult>();

			TestFileResultRepository repository = GetTestFileResultRepository();
			await repository.AddAsync(expectedSession);

			// Act
			TestFileResult actualSession = repository.GetById(expectedSession.Id);

			// Assert
			Assert.That(actualSession, Is.Not.Null, "couldn't find the session");
			string actual = GetAsJson(actualSession);
			string expected = GetAsJson(expectedSession);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task GetSummaries_should_return_sessioninfos()
		{
			// Arrange
			var fixture = new Fixture();
			var session1 = fixture.Create<TestFileResult>();
			var session2 = fixture.Create<TestFileResult>();

			TestFileResultRepository repository = GetTestFileResultRepository();
			await repository.AddAsync(session1);
			await repository.AddAsync(session2);

            // Act
            TestFileResultSummaryCollection summaries = await repository.GetSummaries(It.IsAny<DateTime>());

			// Assert
			Assert.That(summaries.TotalFileResults, Is.EqualTo(2));

			IEnumerable<Guid> ids = summaries.PagedResults.Select(x => x.Id);
			Assert.That(ids, Contains.Item(session1.Id));
			Assert.That(ids, Contains.Item(session2.Id));
		}

		[Test]
		public async Task GetSummaries_should_return_sessioninfo_objects_for_today_only()
		{
			// Arrange
			var fixture = new Fixture();
			var todaySession1 = fixture.Create<TestFileResult>();
			var todaySession2 = fixture.Create<TestFileResult>();
			var otherSession1 = fixture.Create<TestFileResult>();
			var otherSession2 = fixture.Create<TestFileResult>();

			todaySession1.StartTime = DateTime.Today;
			todaySession1.EndTime = todaySession1.StartTime.AddMinutes(5);

			todaySession2.StartTime = DateTime.Today.AddHours(1);
			todaySession2.EndTime = todaySession2.StartTime.AddMinutes(5);

			otherSession1.StartTime = DateTime.Today.AddDays(-2);
			otherSession1.EndTime = otherSession1.StartTime.AddMinutes(10);

			otherSession2.StartTime = DateTime.Today.AddDays(-2);
			otherSession2.EndTime = otherSession2.StartTime.AddMinutes(10);

			TestFileResultRepository repository = GetTestFileResultRepository();
			await repository.AddAsync(todaySession1);
			await repository.AddAsync(todaySession2);
			await repository.AddAsync(otherSession1);
			await repository.AddAsync(otherSession2);

            // Act
            TestFileResultSummaryCollection summaries = await repository.GetSummaries(DateTime.Today);

			// Assert
			Assert.That(summaries.TotalFileResults, Is.EqualTo(2));

			IEnumerable<Guid> ids = summaries.PagedResults.Select(x => x.Id);
			Assert.That(ids, Contains.Item(todaySession1.Id));
			Assert.That(ids, Contains.Item(todaySession2.Id));
		}

		// JSON.NET customisations

		private string GetAsJson(object o)
		{
			return JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings
			{
				// Stop JSON.NET from serializing getter properties
				ContractResolver = new WritablePropertiesOnlyResolver(),

				// Stop JSON.NET putting dates as UTC, as the Z breaks the asserts
				Converters = new List<JsonConverter>() { new JavaScriptDateTimeConverter() }
			});
		}

		private class WritablePropertiesOnlyResolver : DefaultContractResolver
		{
			protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
			{
				IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
				return props.Where(p => p.Writable).ToList();
			}
		}
	}
}
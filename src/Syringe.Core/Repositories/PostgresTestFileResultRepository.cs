using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories
{
	public class PostgresTestFileResultRepository : ITestFileResultRepository
	{
		// docker run  -d --name postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password postgres

		private readonly IDocumentStore _store;

		public PostgresTestFileResultRepository()//IDocumentStore store
		{
			_store = DocumentStore.For(options =>
			{
				options.Connection("host=localhost;database=syringe;password=postgres;username=password");
				options.Schema.For<TestFileResult>().Index(x => x.Id);
			}); ;
			//_store = store;
		}

		public async Task ClearDatabase()
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.DeleteWhere<TestFileResult>(x => true);
				await session.SaveChangesAsync();
			}
		}

		public async Task Add(TestFileResult testFileResult)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.Store(testFileResult);
				await session.SaveChangesAsync();
			}
		}

		public async Task Delete(Guid testFileResultId)
		{
			TestFileResult item = await GetById(testFileResultId);
			if (item != null)
			{
				using (IDocumentSession session = _store.LightweightSession())
				{
					session.Delete(item);
					await session.SaveChangesAsync();
				}
			}
		}

		public Task DeleteBeforeDate(DateTime date)
		{
			throw new NotImplementedException();
		}

		public async Task<TestFileResult> GetById(Guid id)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session.Query<TestFileResult>().FirstOrDefaultAsync(x => x.Id == id);
			}
		}

		public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
		{
			using (IQuerySession session = _store.QuerySession())
			{
				string env = environment?.ToLower();

				Task<int> fileResult = session.Query<TestFileResult>()
										.CountAsync(x => x.StartTime >= fromDateTime
													&& (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env));

				Task<IReadOnlyList<TestFileResult>> testFileCollection = session
					.Query<TestFileResult>()
					.Where(x => x.StartTime >= fromDateTime && (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env))
					.OrderByDescending(x => x.StartTime)
					.Skip((pageNumber - 1) * noOfResults)
					.Take(noOfResults)
					.ToListAsync();

				await Task.WhenAll(fileResult, testFileCollection);

				var collection = new TestFileResultSummaryCollection
				{
					TotalFileResults = fileResult.Result,
					PageNumber = pageNumber,
					NoOfResults = noOfResults,
					PageNumbers = Math.Ceiling((double)fileResult.Result / noOfResults),
					PagedResults = testFileCollection.Result
						.Select(x => new TestFileResultSummary()
						{
							Id = x.Id,
							DateRun = x.StartTime,
							FileName = x.Filename,
							TotalRunTime = x.TotalRunTime,
							TotalPassed = x.TotalTestsPassed,
							TotalFailed = x.TotalTestsFailed,
							TotalRun = x.TotalTestsRun,
							Environment = x.Environment,
							Username = x.Username
						})
				};

				return collection;
			}
		}

		public void Dispose()
		{
		}
	}
}
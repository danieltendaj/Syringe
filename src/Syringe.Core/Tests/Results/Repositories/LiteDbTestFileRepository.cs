using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Syringe.Core.Tests.Results.Repositories
{
	public class LiteDbTestFileRepository : ITestFileResultRepository
	{
		private readonly LiteDatabase _database;
		private readonly string _databasePath;
		private LiteTransaction _transaction;
		private LiteCollection<TestFileResult> _collection;

		public LiteDbTestFileRepository()
		{
			_databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syringe.db");
			_database = new LiteDatabase(_databasePath);
			_collection = _database.GetCollection<TestFileResult>("results");

			// Use a transaction for the lifetime of the instance
			_transaction = _database.BeginTrans();

			// This can be removed in future versions of LiteDB when support for TimeSpan appears.
			BsonMapper.Global.RegisterType<TimeSpan>
			(
				serialize: (timeSpan) => timeSpan.ToString(),
				deserialize: (stringValue) => TimeSpan.Parse(stringValue)
			);
		}

		public async Task AddAsync(TestFileResult testFileResult)
		{
			await Task.Run(() =>
			{
				_collection.Insert(testFileResult);
			});
		}

		public async Task DeleteAsync(Guid testFileResultId)
		{
			await Task.Run(() =>
			{
				_collection.Delete(x => x.Id == testFileResultId);
			});
		}

		public IEnumerable<TestFileResult> GetAll()
		{
			return _collection.FindAll();
		}

		// TODO: Why isn't this async and all the other are?
		public TestFileResult GetById(Guid id)
		{
			return _collection.FindOne(x => x.Id == id);
		}

		public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDate, int pageNumber = 1, int noOfResults = 20, string environment = "")
		{
			// Find/Query seems to need the transaction to be committed before running.
			if (_transaction.State == TransactionState.Started)
			{
				_transaction.Commit();
				_transaction = _database.BeginTrans();
			}

			return await Task.Run(() =>
			{
				var startTimeQuery = Query.GTE("StartTime", fromDate);
				var query = string.IsNullOrEmpty(environment)
										? startTimeQuery
										: Query.And(startTimeQuery, Query.EQ("Environment", environment));

				long totalResults = _collection
										.Find(query)
										.Count();

				List<TestFileResult> testFileCollection = _collection
					.Find(query)
					.OrderByDescending(x => x.StartTime)
					.Skip((pageNumber - 1) * noOfResults)
					.Take(noOfResults)
					.ToList();

				var result = new TestFileResultSummaryCollection
				{
					TotalFileResults = totalResults,
					PageNumber = pageNumber,
					NoOfResults = noOfResults,
					PageNumbers = Math.Ceiling((double)totalResults / noOfResults),
					PagedResults = testFileCollection
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

				return result;
			});
		}

		public void Wipe()
		{
		    if (_database.CollectionExists("results"))
            {
                _database.DropCollection("results");
            }
		}

		public void Dispose()
		{
			if (_transaction.State == TransactionState.Started)
				_transaction.Commit();

			_transaction?.Dispose();
			_database?.Dispose();
		}
	}
}
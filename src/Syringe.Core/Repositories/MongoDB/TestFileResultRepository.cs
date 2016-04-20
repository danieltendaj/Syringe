using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories.MongoDB
{
    public class TestFileResultRepository : ITestFileResultRepository
    {
        private static readonly string MONGDB_COLLECTION_NAME = "TestFileResults";
        private readonly MongoDbConfiguration _mongoDbConfiguration;
	    private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TestFileResult> _collection;

        public TestFileResultRepository(MongoDbConfiguration mongoDbConfiguration)
        {
	        _mongoDbConfiguration = mongoDbConfiguration;
            var mongoClient = new MongoClient(_mongoDbConfiguration.ConnectionString);

            _database = mongoClient.GetDatabase(_mongoDbConfiguration.DatabaseName);
            _collection = _database.GetCollection<TestFileResult>(MONGDB_COLLECTION_NAME);
        }

        public async Task AddAsync(TestFileResult testFileResult)
        {
            await _collection.InsertOneAsync(testFileResult);
        }

        public async Task DeleteAsync(Guid testFileResultId)
        {
            await _collection.DeleteOneAsync(x => x.Id == testFileResultId);
        }

        public TestFileResult GetById(Guid id)
        {
            return _collection.AsQueryable().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<TestFileResultSummary> GetSummaries()
        {
            return _collection.AsQueryable()
                                .ToList()
                                .Select(x => new TestFileResultSummary()
                                {
                                    Id = x.Id,
                                    DateRun = x.StartTime,
                                    FileName = x.Filename,
                                    TotalRunTime = x.TotalRunTime,
                                    TotalPassed = x.TotalTestsPassed,
                                    TotalFailed = x.TotalTestsFailed,
                                    TotalRun = x.TotalTestsRun,
                                    Environment = x.Environment
                                })
                                .OrderByDescending(x => x.DateRun);
        }

        public IEnumerable<TestFileResultSummary> GetSummariesForToday()
        {
            return _collection.AsQueryable()
                                .Where(x => x.StartTime >= DateTime.Today)
                                 .ToList()
                                .Select(x => new TestFileResultSummary()
                                {
                                    Id = x.Id,
                                    DateRun = x.StartTime,
                                    FileName = x.Filename,
                                    TotalRunTime = x.TotalRunTime,
                                    TotalPassed = x.TotalTestsPassed,
                                    TotalFailed = x.TotalTestsFailed,
                                    TotalRun = x.TotalTestsRun,
                                    Environment = x.Environment
                                })
                                .OrderByDescending(x => x.DateRun);
        }

        /// <summary>
        /// Removes all objects from the database.
        /// </summary>
        public void Wipe()
        {
            _database.DropCollectionAsync(MONGDB_COLLECTION_NAME).Wait();
        }
    }
}

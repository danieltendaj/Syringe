using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Syringe.Core.Extensions;
using Syringe.Core.MongoDB;

namespace Syringe.Core.Tests.Results.Repositories
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

        public TestFileResultSummaryCollection GetSummaries(int pageNumber = 1, int noOfResults = 20)
        {
            var collection = new TestFileResultSummaryCollection
            {
                TotalFileResults = _collection.Find(_ => true).CountAsync().Result,
                PageNumber = pageNumber,
                PagedResults = _collection
                    .Find(_ => true)
                    .Sort(Builders<TestFileResult>.Sort.Descending("DateRun"))
                    .Skip((pageNumber - 1)*noOfResults)
                    .Limit(noOfResults)
                    .ToListAsync()
                    .Result
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
            };

            return collection;
        }

        public TestFileResultSummaryCollection GetSummariesForToday(int pageNumber = 1, int noOfResults = 20)
        {
            var collection = new TestFileResultSummaryCollection
            {
                TotalFileResults = _collection.Find(x => x.StartTime >= DateTime.Today).CountAsync().Result,
                PageNumber = pageNumber,
                PagedResults = _collection
                    .Find(x => x.StartTime >= DateTime.Today)
                    .Sort(Builders<TestFileResult>.Sort.Descending("DateRun"))
                    .Skip((pageNumber - 1)*noOfResults)
                    .Limit(noOfResults)
                    .ToListAsync()
                    .Result
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
            };

            return collection;
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

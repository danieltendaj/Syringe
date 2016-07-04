using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.MongoDB;

namespace Syringe.Core.Tests.Results.Repositories
{
    public class MongoTestFileResultRepository : ITestFileResultRepository
    {
        private static readonly string MONGDB_COLLECTION_NAME = "TestFileResults";
        private readonly MongoDbConfiguration _mongoDbConfiguration;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TestFileResult> _collection;

        public MongoTestFileResultRepository(MongoDbConfiguration mongoDbConfiguration)
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

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDate, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            // Ensure TestFileResult has indexes on the date it was run and the environment.
            // These index commands don't rebuild the index, they just send the command.
            await _collection.Indexes.CreateOneAsync(Builders<TestFileResult>.IndexKeys.Ascending(x => x.StartTime));
            await _collection.Indexes.CreateOneAsync(Builders<TestFileResult>.IndexKeys.Ascending(x => x.Environment));
            string env = environment?.ToLower();
            Task<long> fileResult = _collection.CountAsync(x => x.StartTime >= fromDate && (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env));

            Task<List<TestFileResult>> testFileCollection = _collection
                .Find(x => x.StartTime >= fromDate && (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env))
                .Sort(Builders<TestFileResult>.Sort.Descending(x => x.StartTime))
                .Skip((pageNumber - 1) * noOfResults)
                .Limit(noOfResults)
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

        /// <summary>
        /// Removes all objects from the database.
        /// </summary>
        public void Wipe()
        {
            _database.DropCollectionAsync(MONGDB_COLLECTION_NAME).Wait();
        }

        public void Dispose()
        { }
    }
}

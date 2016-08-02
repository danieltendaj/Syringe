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
        private readonly object _lock = new object();
        private readonly string _databaseLocation;
        private LiteDatabase _database;

        public LiteDbTestFileRepository()
        {
            _databaseLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syringe.db");
        }

        public async Task AddAsync(TestFileResult testFileResult)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    LiteCollection<TestFileResult> collection = GetCollection();
                    collection.Insert(testFileResult);
                }
            });
        }

        public async Task DeleteAsync(Guid testFileResultId)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    LiteCollection<TestFileResult> collection = GetCollection();
                    collection.Delete(x => x.Id == testFileResultId);
                }
            });
        }

        // TODO: Why isn't this async and all the other are?
        public TestFileResult GetById(Guid id)
        {
            lock (_lock)
            {
                LiteCollection<TestFileResult> collection = GetCollection();
                return collection.FindOne(x => x.Id == id);
            }
        }

        public void Wipe()
        {
            lock (_lock)
            {
                _database?.DropCollection("results");
            }
        }

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDate, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    LiteCollection<TestFileResult> collection = GetCollection();

                    var startTimeQuery = Query.GTE("StartTime", fromDate);
                    var query = string.IsNullOrEmpty(environment) 
                                            ? startTimeQuery
                                            : Query.And(startTimeQuery, Query.EQ("Environment", environment));
                    
                    long totalResults = collection
                                            .Find(query)
                                            .Count();
            
                    List<TestFileResult> testFileCollection = collection
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
                }
            });
        }

        private LiteCollection<TestFileResult> GetCollection()
        {
            if (_database == null)
            {
                _database = new LiteDatabase(_databaseLocation);
            }

            var collection = _database.GetCollection<TestFileResult>("results");
            return collection;
        }

        public void Dispose()
        {
            if (_database != null)
            {
                lock (_lock)
                {
                    _database.Dispose();
                }
            }
        }
    }
}
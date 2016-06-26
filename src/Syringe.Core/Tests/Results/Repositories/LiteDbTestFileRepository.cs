using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Syringe.Core.Tests.Results.Repositories
{
    // TODO: No async methods, so wrapping them up
    // TODO: I think the IDs are wrong, should be moved to BSONId?
    public class LiteDbTestFileRepository : ITestFileResultRepository
    {
        private readonly object _lock = new Object();
        private readonly string _databaseLocation;
        private LiteDatabase _database;

        public LiteDbTestFileRepository()
        {
            _databaseLocation = Path.Combine(System.Environment.CurrentDirectory, "syringe.db");
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
                    //x => x.StartTime >= fromDate && (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env)
                    //string env = environment ?? string.Empty;//?.ToLower();
                    //long fileResult = collection
                    //                    .Find(Query.And(
                    //                            Query.GTE("StartTime", fromDate),
                    //                            Query.Or(Query.EQ("Environment", env), Query.EQ("Environment", string.Empty))
                    //                        ))
                    //                    .Count();

                    string env = environment?.ToLower();
                    long totalResults = collection
                                            .Find(Query.All())
                                            .Count();
                    //.Find(x => x.StartTime >= fromDate && (string.IsNullOrEmpty(environment) || x.Environment.ToLower() == env))
                    //.Count();


                    List<TestFileResult> testFileCollection = collection
                        .Find(Query.All())
                        //.Find(x => x.StartTime >= fromDate && (string.IsNullOrEmpty(env) || x.Environment == env))
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
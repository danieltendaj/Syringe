using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Syringe.Core.Tests.Results.Repositories
{
    /// <summary>
    /// LiteDb isn't multithreaded yet, so open and close connections on all db calls for now
    /// https://github.com/mbdavid/LiteDB/issues/275
    /// </summary>
    public class LiteDbTestFileRepository : ITestFileResultRepository
    {
        private readonly string _databasePath;

        static LiteDbTestFileRepository()
        {
            // This can be removed in future versions of LiteDB when support for TimeSpan appears.
            BsonMapper.Global.RegisterType<TimeSpan>
            (
                serialize: (timeSpan) => timeSpan.ToString(),
                deserialize: (stringValue) => TimeSpan.Parse(stringValue)
            );
        }

        public LiteDbTestFileRepository()
        {
            _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syringe.db");
        }

        public async Task AddAsync(TestFileResult testFileResult)
        {
            await Task.Run(() =>
            {
                using (var db = new LiteDatabase(_databasePath))
                {
                    GetCollection(db).Insert(testFileResult);
                }
            });
        }

        public async Task DeleteAsync(Guid testFileResultId)
        {
            await Task.Run(() =>
            {
                using (var db = new LiteDatabase(_databasePath))
                {
                    GetCollection(db).Delete(x => x.Id == testFileResultId);
                }
            });
        }

        public IEnumerable<TestFileResult> GetAll()
        {
            using (var db = new LiteDatabase(_databasePath))
            {
                return GetCollection(db).FindAll();
            }
        }

        // TODO: Why isn't this async and all the other are?
        public TestFileResult GetById(Guid id)
        {
            using (var db = new LiteDatabase(_databasePath))
            {
                return GetCollection(db).FindOne(x => x.Id == id);
            }
        }

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDate, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return await Task.Run(() =>
            {
                var startTimeQuery = Query.GTE("StartTime", fromDate);
                var query = string.IsNullOrEmpty(environment)
                                        ? startTimeQuery
                                        : Query.And(startTimeQuery, Query.EQ("Environment", environment));

                List<TestFileResult> testFileCollection;
                long totalResults;

                using (var db = new LiteDatabase(_databasePath))
                {
                    var collection = GetCollection(db);
                    totalResults = collection
                        .Find(query)
                        .Count();

                    testFileCollection = collection
                       .Find(query)
                       .OrderByDescending(x => x.StartTime)
                       .Skip((pageNumber - 1) * noOfResults)
                       .Take(noOfResults)
                       .ToList();
                }

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
            using (var db = new LiteDatabase(_databasePath))
            {
                if (db.CollectionExists("results"))
                {
                    db.DropCollection("results");
                }
            }
        }

        private LiteCollection<TestFileResult> GetCollection(LiteDatabase liteDatabase)
        {
            return liteDatabase.GetCollection<TestFileResult>("results");
        }

        public void Dispose()
        {

        }
    }
}
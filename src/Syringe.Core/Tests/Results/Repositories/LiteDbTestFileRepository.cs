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
        private readonly string _database;

        public LiteDbTestFileRepository()
        {
            _database = Path.Combine(System.Environment.CurrentDirectory, "syringe.db");
        }

        public async Task AddAsync(TestFileResult testFileResult)
        {
            await Task.Run(() =>
            {
                using (var db = new LiteDatabase(_database))
                {
                    //testFileResult.Id = Guid.NewGuid(); //TODO: we might need this
                    LiteCollection<TestFileResult> collection = GetCollection(db);
                    collection.Insert(testFileResult);
                }
            });
        }

        public async Task DeleteAsync(Guid testFileResultId)
        {
            await Task.Run(() =>
            {
                using (var db = new LiteDatabase(_database))
                {
                    LiteCollection<TestFileResult> collection = GetCollection(db);
                    collection.Delete(x => x.Id == testFileResultId);
                }
            });
        }

        // TODO: Why isn't this async and all the other are?
        public TestFileResult GetById(Guid id)
        {
            using (var db = new LiteDatabase(_database))
            {
                LiteCollection<TestFileResult> collection = GetCollection(db);
                return collection.FindOne(x => x.Id == id);
            }
        }

        public void Wipe()
        {
            throw new NotImplementedException();
        }

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDate, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return await Task.Run(() =>
            {
                using (var db = new LiteDatabase(_database))
                {
                    LiteCollection<TestFileResult> collection = GetCollection(db);
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

        private static LiteCollection<TestFileResult> GetCollection(LiteDatabase db)
        {
            var collection = db.GetCollection<TestFileResult>("results");
            return collection;
        }
    }
}
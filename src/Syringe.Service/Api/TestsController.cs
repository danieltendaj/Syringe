using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Api
{
    public class TestsController : ApiController, ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestFileResultRepository _testFileResultRepository;

        public TestsController(ITestRepository testRepository, ITestFileResultRepository testFileResultRepository)
        {
            _testRepository = testRepository;
            _testFileResultRepository = testFileResultRepository;
        }

        [Route("api/tests/ListFiles")]
        [HttpGet]
        public IEnumerable<string> ListFiles()
        {
            return _testRepository.ListFiles();
        }

        [Route("api/tests/GetTest")]
        [HttpGet]
        public Test GetTest(string filename, string branchName, int position)
        {
            return _testRepository.GetTest(filename, branchName, position);
        }

        [Route("api/tests/GetTestFile")]
        [HttpGet]
        public TestFile GetTestFile(string filename, string branchName)
        {
            return _testRepository.GetTestFile(filename, branchName);
        }
        [Route("api/tests/GetXml")]
        [HttpGet]
        public string GetXml(string filename, string branchName)
        {
            return _testRepository.GetXml(filename, branchName);
        }

        [Route("api/tests/EditTest")]
        [HttpPost]
        public bool EditTest([FromBody]Test test, [FromUri]string branchName)
        {
            return _testRepository.SaveTest(test, branchName);
        }

        [Route("api/tests/CreateTest")]
        [HttpPost]
        public bool CreateTest([FromBody]Test test, [FromUri]string branchName)
        {
            return _testRepository.CreateTest(test, branchName);
        }

        [Route("api/tests/DeleteTest")]
        [HttpPost]
        public bool DeleteTest(int position, string fileName, string branchName)
        {
            return _testRepository.DeleteTest(position, fileName, branchName);
        }

        [Route("api/tests/CreateTestFile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]TestFile testFile, [FromUri]string branchName)
        {
            return _testRepository.CreateTestFile(testFile, branchName);
        }

        [Route("api/tests/UpdateTestVariables")]
        [HttpPost]
        public bool UpdateTestVariables([FromBody]TestFile testFile, [FromUri]string branchName)
        {
            return _testRepository.UpdateTestVariables(testFile, branchName);
        }

        [Route("api/tests/GetSummariesForToday")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummariesForToday()
        {
            return _testFileResultRepository.GetSummariesForToday();
        }

        [Route("api/tests/GetSummaries")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummaries()
        {
            return _testFileResultRepository.GetSummaries();
        }

        [Route("api/tests/GetById")]
        [HttpGet]
        public TestFileResult GetResultById(Guid id)
        {
            return _testFileResultRepository.GetById(id);
        }

        [Route("api/tests/DeleteResultAsync")]
        [HttpPost]
        public Task DeleteResultAsync(Guid id)
        {
            return _testFileResultRepository.DeleteAsync(id);
        }

        [Route("api/tests/DeleteFile")]
        [HttpPost]
        public bool DeleteFile(string fileName, string branchName)
        {
            return _testRepository.DeleteFile(fileName,branchName);
        }
    }
}
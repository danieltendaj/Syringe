using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

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
        public Test GetTest(string filename, int position)
        {
            return _testRepository.GetTest(filename, position);
        }

        [Route("api/tests/GetTestFile")]
        [HttpGet]
        public TestFile GetTestFile(string filename)
        {
            return _testRepository.GetTestFile(filename);
        }

        [Route("api/tests/GetRawFile")]
        [HttpGet]
        public string GetRawFile(string filename)
        {
            return _testRepository.GetRawFile(filename);
        }

        [Route("api/tests/EditTest")]
        [HttpPost]
        public bool EditTest([FromBody]Test test)
        {
            return _testRepository.SaveTest(test);
        }

        [Route("api/tests/CreateTest")]
        [HttpPost]
        public bool CreateTest([FromBody]Test test)
        {
            return _testRepository.CreateTest(test);
        }

        [Route("api/tests/DeleteTest")]
        [HttpPost]
        public bool DeleteTest(int position, string fileName)
        {
            return _testRepository.DeleteTest(position, fileName);
        }

        [Route("api/tests/CopyTest")]
        [HttpPost]
        public bool CopyTest(int position, string fileName)
        {
            throw new NotImplementedException();
        }

        [Route("api/tests/CreateTestFile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]TestFile testFile)
        {
            return _testRepository.CreateTestFile(testFile);
        }

        [Route("api/tests/UpdateTestVariables")]
        [HttpPost]
        public bool UpdateTestVariables([FromBody]TestFile testFile)
        {
            return _testRepository.UpdateTestVariables(testFile);
        }

        [Route("api/tests/GetSummaries")]
        [HttpGet]
        public Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20)
        {
            return _testFileResultRepository.GetSummaries(fromDateTime, pageNumber, noOfResults);
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
        public bool DeleteFile(string fileName)
        {
            return _testRepository.DeleteFile(fileName);
        }
    }
}
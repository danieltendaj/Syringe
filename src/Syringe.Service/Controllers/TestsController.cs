using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Service.Controllers
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

        [Route("api/testfiles")]
        [HttpGet]
        public IEnumerable<string> ListFiles()
        {
            return _testRepository.ListFiles();
        }
        
        [Route("api/testfile")]
        [HttpGet]
        public TestFile GetTestFile(string filename)
        {
            return _testRepository.GetTestFile(filename);
        }

        [Route("api/testfile/raw")]
        [HttpGet]
        public string GetRawFile(string filename)
        {
            return _testRepository.GetRawFile(filename);
        }

        [Route("api/testfile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]TestFile testFile)
        {
            return _testRepository.CreateTestFile(testFile);
        }

        [Route("api/testfile/copy")]
        [HttpPost]
        public bool CopyTestFile(string sourceFileName, string targetFileName)
        {
            TestFile testFile = _testRepository.GetTestFile(sourceFileName);
            testFile.Filename = targetFileName;

            bool result = _testRepository.CreateTestFile(testFile);
            return result;
        }

        [Route("api/testfile/variables")]
        [HttpPost]
        public bool UpdateTestVariables([FromBody]TestFile testFile)
        {
            return _testRepository.UpdateTestVariables(testFile);
        }

        [Route("api/testfile")]
        [HttpDelete]
        public bool DeleteFile(string fileName)
        {
            return _testRepository.DeleteFile(fileName);
        }

        [Route("api/test")]
        [HttpPatch]
        public bool EditTest(string filename, int position, [FromBody]Test test)
        {
            return _testRepository.SaveTest(filename, position, test);
        }
        
        [Route("api/test")]
        [HttpPost]
        public bool CreateTest(string filename, [FromBody]Test test)
        {
            return _testRepository.CreateTest(filename, test);
        }

        [Route("api/test")]
        [HttpDelete]
        public bool DeleteTest(int position, string fileName)
        {
            return _testRepository.DeleteTest(position, fileName);
        }

        [Route("api/test/copy")]
        [HttpPost]
        public bool CopyTest(int position, string fileName)
        {
            Test test = _testRepository.GetTest(fileName, position);
            test.Description = $"Copy of {test.Description}";
            return _testRepository.CreateTest(fileName, test);
        }

        [Route("api/test/results")]
        [HttpGet]
        public Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return _testFileResultRepository.GetSummaries(fromDateTime, pageNumber, noOfResults, environment);
        }

        [Route("api/test/result")]
        [HttpGet]
        public TestFileResult GetResultById(Guid id)
        {
            return _testFileResultRepository.GetById(id);
        }

        [Route("api/test/result")]
        [HttpDelete]
        public bool DeleteResult(Guid id)
        {
            _testFileResultRepository.Delete(id).Wait();
            return true;
        }

	    [Route("api/test/reorder")]
        [HttpPost]
        public bool ReorderTests(string fileName, [FromBody]IEnumerable<TestPosition> tests)
        {
            TestFile testFile = _testRepository.GetTestFile(fileName);

            var newOrderList = new List<Test>();

            List<TestPosition> testPositions = tests.ToList();
            foreach (var test in testPositions)
            {
                newOrderList.Add(testFile.Tests.ElementAtOrDefault(test.OriginalPostion));
            }

            testFile.Tests = newOrderList;
            
            return _testRepository.UpdateTests(testFile);
        }
    }
}
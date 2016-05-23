using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Client.RestSharpHelpers;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Client
{
    public class TestsClient : ITestService
    {
        internal const string RESOURCE_PATH = "/api/tests";
        private readonly string _serviceUrl;
        private readonly IRestSharpClientFactory _clientFactory;
        private readonly RestSharpHelper _restSharpHelper;

        public TestsClient(string serviceUrl, IRestSharpClientFactory clientFactory)
        {
            _serviceUrl = serviceUrl;
            _clientFactory = clientFactory;
            _restSharpHelper = new RestSharpHelper(RESOURCE_PATH);
        }

        public IEnumerable<string> ListFiles()
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("ListFiles");

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<IEnumerable<string>>(response);
        }

        public Test GetTest(string filename, int position)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetTest");
            request.AddParameter("filename", filename);
            request.AddParameter("position", position);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<Test>(response);
        }

        public TestFile GetTestFile(string filename)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetTestFile");
            request.AddParameter("filename", filename);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<TestFile>(response);
        }

        public string GetRawFile(string filename)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetRawFile");
            request.AddParameter("filename", filename);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<string>(response);
        }

        public bool EditTest(Test test)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("EditTest");
            request.Method = Method.POST;
            request.AddJsonBody(test);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool EditTest(string filename, int position, Test test)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("EditTest");
            request.Method = Method.POST;
            request.AddJsonBody(test);
            request.AddQueryParameter("filename", filename);
            request.AddQueryParameter("position", Convert.ToString(position));

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CreateTest(Test test)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("CreateTest");
            request.Method = Method.POST;
            request.AddJsonBody(test);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool DeleteTest(int position, string fileName)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("DeleteTest");
            request.Method = Method.POST;
            request.AddQueryParameter("position", position.ToString());
            request.AddQueryParameter("fileName", fileName);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CopyTest(int position, string fileName)
        {
            var client = _clientFactory.Create(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("CopyTest");
            request.Method = Method.POST;
            request.AddQueryParameter("position", Convert.ToString(position));
            request.AddQueryParameter("fileName", fileName);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CreateTestFile(TestFile testFile)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("CreateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("fileName", testFile.Filename);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CopyTestFile(string sourceFileName, string targetFileName)
        {
            var client = _clientFactory.Create(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("CopyTestFile");
            request.Method = Method.POST;
            request.AddQueryParameter("sourceFileName", sourceFileName);
            request.AddQueryParameter("targetFileName", targetFileName);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool UpdateTestVariables(TestFile testFile)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("UpdateTestVariables");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetSummaries");
            request.Method = Method.GET;
            request.AddQueryParameter("pageNumber", pageNumber.ToString());
            request.AddQueryParameter("noOfResults", noOfResults.ToString());
            request.AddQueryParameter("fromDateTime", fromDateTime.ToString(CultureInfo.InvariantCulture));
            IRestResponse response = await client.ExecuteGetTaskAsync(request);
            return _restSharpHelper.DeserializeOrThrow<TestFileResultSummaryCollection>(response);
        }

        public TestFileResult GetResultById(Guid id)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetById");
            request.Method = Method.GET;
            request.AddQueryParameter("id", id.ToString());
            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<TestFileResult>(response);
        }

        public Task DeleteResultAsync(Guid id)
        {
            var client = new RestClient(_serviceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("DeleteResultAsync");
            request.Method = Method.POST;
            request.AddQueryParameter("id", id.ToString());

            return client.ExecutePostTaskAsync(request);
        }

        public bool DeleteFile(string fileName)
        {
            var client = new RestClient(_serviceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("DeleteFile");
            request.Method = Method.POST;
            request.AddQueryParameter("filename", fileName);
            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }
    }
}
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
        internal const string RESOURCE_PATH = "/api/";
        internal readonly string ServiceUrl;
        private readonly IRestSharpClientFactory _clientFactory;
        private readonly RestSharpHelper _restSharpHelper;

        public TestsClient(string serviceUrl, IRestSharpClientFactory clientFactory)
        {
            ServiceUrl = serviceUrl;
            _clientFactory = clientFactory;
            _restSharpHelper = new RestSharpHelper(RESOURCE_PATH);
        }

        public IEnumerable<string> ListFiles()
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfiles");

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<IEnumerable<string>>(response);
        }
        
        public TestFile GetTestFile(string filename)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfile");
            request.Method = Method.GET;
            request.AddParameter("filename", filename);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<TestFile>(response);
        }

        public string GetRawFile(string filename)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfile/raw");
            request.AddParameter("filename", filename);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<string>(response);
        }

        public bool EditTest(string filename, int position, Test test)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test");
            request.Method = Method.PATCH;
            request.AddJsonBody(test);
            request.AddQueryParameter("filename", filename);
            request.AddQueryParameter("position", Convert.ToString(position));

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CreateTest(string filename, Test test)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test");
            request.Method = Method.POST;
            request.AddJsonBody(test);
            request.AddQueryParameter("filename", filename);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool DeleteTest(int position, string fileName)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test");
            request.Method = Method.DELETE;
            request.AddQueryParameter("position", position.ToString());
            request.AddQueryParameter("fileName", fileName);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CopyTest(int position, string fileName)
        {
            var client = _clientFactory.Create(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test/copy");
            request.Method = Method.POST;
            request.AddQueryParameter("position", Convert.ToString(position));
            request.AddQueryParameter("fileName", fileName);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CreateTestFile(TestFile testFile)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("filename", testFile.Filename);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool CopyTestFile(string sourceFileName, string targetFileName)
        {
            var client = _clientFactory.Create(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfile/copy");
            request.Method = Method.POST;
            request.AddQueryParameter("sourceFileName", sourceFileName);
            request.AddQueryParameter("targetFileName", targetFileName);

            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool UpdateTestVariables(TestFile testFile)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("testfile/variables");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test/results");
            request.Method = Method.GET;
            request.AddQueryParameter("pageNumber", pageNumber.ToString());
            request.AddQueryParameter("noOfResults", noOfResults.ToString());
            request.AddQueryParameter("fromDateTime", fromDateTime.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("environment", environment);
            IRestResponse response = await client.ExecuteGetTaskAsync(request);
            return _restSharpHelper.DeserializeOrThrow<TestFileResultSummaryCollection>(response);
        }

        public TestFileResult GetResultById(Guid id)
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("test/result");
            request.Method = Method.GET;
            request.AddQueryParameter("id", id.ToString());
            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<TestFileResult>(response);
        }

        public bool DeleteResult(Guid id)
        {
            var client = new RestClient(ServiceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("test/result");
            request.Method = Method.DELETE;
            request.AddQueryParameter("id", id.ToString());

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public bool Reorder(TestFileOrder testFileToReorder)
        {
            var client = new RestClient(ServiceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("test/reorder");
            request.Method = Method.POST;
            request.AddJsonBody(testFileToReorder);
            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }

        public TestFileOrder GetTestFileOrder(string testFile)
        {
            var client = new RestClient(ServiceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("test/reorder");
            request.Method = Method.GET;
            request.AddQueryParameter("testFile",testFile);
            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<TestFileOrder>(response);
        }

        public bool DeleteFile(string fileName)
        {
            var client = new RestClient(ServiceUrl);

            IRestRequest request = _restSharpHelper.CreateRequest("testfile");
            request.Method = Method.DELETE;
            request.AddQueryParameter("filename", fileName);
            IRestResponse response = client.Execute(request);

            return _restSharpHelper.DeserializeOrThrow<bool>(response);
        }
    }
}
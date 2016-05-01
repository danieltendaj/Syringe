using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Client
{
	public class TestsClient : ITestService
	{
		private readonly string _serviceUrl;
		private readonly RestSharpHelper _restSharpHelper;

		public TestsClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/tests");
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

	    public string GetXml(string filename)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("GetXml");
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
            IRestResponse response =  await client.ExecuteGetTaskAsync(request);
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
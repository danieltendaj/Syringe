using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Variables;

namespace Syringe.Service.Parallel
{
    public class TestFileAssembler : ITestFileAssembler
    {
        private readonly ITestRepository _testRepository;

        public TestFileAssembler(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public TestFile AssembleTestFile(string testFileName, string environment)
        {
            string[] fileNameSplit = (testFileName ?? string.Empty).Split('?');
            TestFile testFile = _testRepository.GetTestFile(fileNameSplit[0]);

            if (testFile != null && fileNameSplit.Length > 1)
            {
                ApplyVariableOverrides(testFile, fileNameSplit[1], environment);
            }

            return testFile;
        }

        private static void ApplyVariableOverrides(TestFile testFile, string queryString, string environment)
        {
            NameValueCollection queryStringCollection = ParseQueryString(queryString);
            if (queryStringCollection.Count > 0)
            {
                var variablesToRemove = new List<Variable>();
                foreach (string variableName in queryStringCollection.Keys)
                {
                    variablesToRemove.AddRange(testFile.Variables.Where(x => x.Name == variableName));

                    testFile.Variables.Add(new Variable(variableName, queryStringCollection[variableName], environment));
                }

                foreach (var variable in variablesToRemove)
                {
                    testFile.Variables.Remove(variable);
                }
            }
        }

        private static NameValueCollection ParseQueryString(string queryString)
        {
            var queryParameters = new NameValueCollection();

            string[] querySegments = queryString.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 1)
                {
                    string key = WebUtility.UrlDecode(parts[0].Trim('?', ' '));
                    string val = WebUtility.UrlDecode(parts[1].Trim());

                    queryParameters.Set(key, val);
                }
            }

            return queryParameters;
        }
    }
}
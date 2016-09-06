using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            var uriInfo = new UriBuilder(testFileName);
            TestFile testFile = _testRepository.GetTestFile(uriInfo.Host);

            if (testFile != null)
            {
                ApplyVariableOverrides(uriInfo, testFile, environment);
            }

            return testFile;
        }

        private static void ApplyVariableOverrides(UriBuilder uriInfo, TestFile testFile, string environment)
        {
            var queryString = uriInfo.Uri.ParseQueryString();
            if (queryString.Count > 0)
            {
                var variablesToRemove = new List<Variable>();
                foreach (string variableName in queryString.Keys)
                {
                    variablesToRemove.AddRange(testFile.Variables.Where(x => x.Name == variableName));

                    testFile.Variables.Add(new Variable(variableName, queryString[variableName], environment));
                }

                foreach (var variable in variablesToRemove)
                {
                    testFile.Variables.Remove(variable);
                }
            }
        }
    }
}
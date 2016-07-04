using System;
using System.Collections.Generic;
using System.IO;
using Syringe.Client;
using Syringe.Client.RestSharpHelpers;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.ClientAndService
{
    public class Helpers
    {
        public static string GetJsonFilename()
        {
            Guid guid = Guid.NewGuid();
            return $"{guid}.json";
        }

        public static string GetFullPath(string filename)
        {
            return Path.Combine(ServiceStarter.TestFilesDirectoryPath, filename);
        }

        public static TestsClient CreateTestsClient()
        {
            var client = new TestsClient(ServiceStarter.BaseUrl, new RestSharpClientFactory());
            return client;
        }

        public static TestFile CreateTestFileAndTest(TestsClient client)
        {
            string filename = GetJsonFilename();
            var test1 = new Test()
            {
                Assertions = new List<Assertion>(),
                AvailableVariables = new List<Variable>(),
                CapturedVariables = new List<CapturedVariable>(),
                Headers = new List<HeaderItem>(),
                Description = "short desc 1",
                Method = "POST",
                Url = "url 1"
            };

            var test2 = new Test()
            {
                Assertions = new List<Assertion>(),
                AvailableVariables = new List<Variable>(),
                CapturedVariables = new List<CapturedVariable>(),
                Headers = new List<HeaderItem>(),
                Description = "short desc 2",
                Method = "POST",
                Url = "url 2"
            };

            var testFile = new TestFile { Filename = filename };
            client.CreateTestFile(testFile);
            client.CreateTest(filename, test1);
            client.CreateTest(filename, test2);

            var tests = new List<Test>()
            {
                test1,
                test2
            };
            testFile.Tests = tests;

            return testFile;
        }
    }
}
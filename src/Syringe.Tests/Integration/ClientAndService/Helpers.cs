using System;
using System.Collections.Generic;
using System.IO;
using Syringe.Client;
using Syringe.Core.Tests;

namespace Syringe.Tests.Integration.ClientAndService
{
	public class Helpers
	{
		public static string GetXmlFilename()
		{
			Guid guid = Guid.NewGuid();
			return $"{guid}.xml";
		}

		public static string GetFullPath(string filename)
		{
			return Path.Combine(ServiceConfig.XmlDirectoryPath, filename);
		}

		public static TestsClient CreateTestsClient()
		{
			var client = new TestsClient(ServiceConfig.BaseUrl);
			return client;
		}

		public static TestFile CreateTestFileAndTest(TestsClient client)
		{
			string filename = GetXmlFilename();
			var test1 = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message 2",
				Headers = new List<HeaderItem>(),
				ShortDescription = "short desc 1",
				Method = "POST",
				Url = "url 1"
			};

			var test2 = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message 2",
				Headers = new List<HeaderItem>(),
				ShortDescription = "short desc 2",
				Method = "POST",
				Url = "url 2"
			};

			var testFile = new TestFile() { Filename = filename };
			client.CreateTestFile(testFile, ServiceConfig.BranchName);
			client.CreateTest(test1, ServiceConfig.BranchName);
			client.CreateTest(test2, ServiceConfig.BranchName);

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
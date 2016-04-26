using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Repositories.XML.Reader;
using Syringe.Core.Runner;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Integration.Xml
{
	public class TestFileRunnerTests
	{
		public static string XmlExamplesFolder = typeof(TestFileRunnerTests).Namespace + ".XmlExamples.Runner.";

		private ITestFileResultRepository GetRepository()
		{
			return new TestFileResultRepositoryMock();
		}

	    [Test]
	    public void should_throw_ArgumentNullException_when_httpclient_is_null()
	    {
	        Assert.Throws<ArgumentNullException>(() => new TestFileRunner(null, new TestFileResultRepositoryMock()));
	    }

        [Test]
        public void should_throw_ArgumentNullException_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TestFileRunner(It.IsAny<IHttpClient>(), null));
        }

        [Test]
		public async Task should_parse_capturedvariables()
		{
			// Arrange
			var httpClient = new HttpClient(new RestClient());

			string xml = TestHelpers.ReadEmbeddedFile("capturedvariables.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new TestFileReader();
			TestFile testFile = reader.Read(stringReader);
			var runner = new TestFileRunner(httpClient, GetRepository());

			// Act
			TestFileResult result = await runner.RunAsync(testFile);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.TestResults.Count, Is.EqualTo(2));
		}
	}
}

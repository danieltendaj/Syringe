using System.IO;
using Syringe.Core.Repositories.XML.Reader;
using Syringe.Core.Tests;

namespace Syringe.Tests.StubsMocks
{
	public class TestFileReaderMock : ITestFileReader
	{
		public TestFile TestFile { get; set; }

		public TestFile Read(TextReader textReader)
		{
			return TestFile;
		}
	}
}
using System.IO;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;

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
using System.IO;

namespace Syringe.Core.Tests.Repositories
{
	public interface ITestFileReader
    {
		TestFile Read(TextReader textReader);
    }
}
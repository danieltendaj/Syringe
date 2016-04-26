using Syringe.Core.Tests;

namespace Syringe.Core.Repositories.XML.Writer
{
	public interface ITestFileWriter
	{
		string Write(TestFile testFile);
	}
}
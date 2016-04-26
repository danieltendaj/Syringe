using System.IO;
using Syringe.Core.Tests;

namespace Syringe.Core.Repositories.XML.Reader
{
	public interface ITestFileReader
    {
		TestFile Read(TextReader textReader);
    }
}
using System.IO;
using Newtonsoft.Json;

namespace Syringe.Core.Tests.Repositories.Json.Reader
{
    public class TestFileReader : ITestFileReader
    {
        public TestFile Read(TextReader textReader)
        {
            string data = textReader.ReadToEnd();
            return JsonConvert.DeserializeObject<TestFile>(data);
        }
    }
}
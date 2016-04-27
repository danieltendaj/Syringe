using Newtonsoft.Json;

namespace Syringe.Core.Tests.Repositories.Json.Writer
{
    public class TestFileWriter : ITestFileWriter
    {
        public string Write(TestFile testFile)
        {
            return JsonConvert.SerializeObject(testFile, Formatting.Indented);
        }
    }
}
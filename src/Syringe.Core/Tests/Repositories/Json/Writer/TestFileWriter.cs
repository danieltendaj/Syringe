using Newtonsoft.Json;

namespace Syringe.Core.Tests.Repositories.Json.Writer
{
    public class TestFileWriter : ITestFileWriter
    {
        private readonly SerializationContract _serializationContract;

        public TestFileWriter(SerializationContract serializationContract)
        {
            _serializationContract = serializationContract;
        }

        public string Write(TestFile testFile)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = _serializationContract
            };

            return JsonConvert.SerializeObject(testFile, Formatting.Indented, settings);
        }
    }
}
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;

namespace Syringe.Tests.StubsMocks
{
    public class TestFileWriterStub : ITestFileWriter
    {
        public string Write_Result { get; set; }
        public TestFile Write_Value { get; private set; }

        public string Write(TestFile testFile)
        {
            Write_Value = testFile;
            return Write_Result;
        }
    }
}
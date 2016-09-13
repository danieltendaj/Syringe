using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Runner.Logging;

namespace Syringe.Tests.StubsMocks
{
    public class TestFileRunnerLogFactoryMock : ITestFileRunnerLoggerFactory
    {
        public ITestFileRunnerLogger CreateLogger()
        {
            return new TestFileRunnerLoggerMock();
        }
    }
}

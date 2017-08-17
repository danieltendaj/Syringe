using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;

namespace Syringe.Tests.StubsMocks
{
	public class HealthCheckMock : IHealthCheck
	{
		public bool ThrowsException { get; set; }

		public void CheckWebConfiguration()
		{
			if (ThrowsException)
			{
				throw new HealthCheckException("Message");
			}
		}

		public void CheckServiceConfiguration()
		{
			if (ThrowsException)
			{
				throw new HealthCheckException("Message");
			}
		}

		public void CheckServiceSwaggerIsRunning()
		{
			if (ThrowsException)
			{
				throw new HealthCheckException("Message");
			}
		}
	}
}
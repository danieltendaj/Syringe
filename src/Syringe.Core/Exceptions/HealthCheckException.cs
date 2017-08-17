using System;

namespace Syringe.Core.Exceptions
{
	public class HealthCheckException : Exception
	{
		public HealthCheckException(string message, string name) : base(message)
		{
		}

		public HealthCheckException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}
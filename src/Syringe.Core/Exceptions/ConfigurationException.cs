using System;

namespace Syringe.Core.Exceptions
{
	public class ConfigurationException : Exception
	{
		public ConfigurationException(string message, string args) : base(string.Format(message, args))
		{
		}

		public ConfigurationException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}

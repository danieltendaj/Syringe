using System;

namespace Syringe.Core.Exceptions
{
	public class SyringeException : Exception
	{
		public SyringeException(string message) : base(message)
		{
		}
	}
}
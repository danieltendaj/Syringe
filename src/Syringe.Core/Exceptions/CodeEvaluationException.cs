using System;

namespace Syringe.Core.Exceptions
{
	public class CodeEvaluationException : Exception
	{
		public CodeEvaluationException(string message) : base(message)
		{
		}

		public CodeEvaluationException(Exception inner, string message, params object[] args) : base(string.Format(message, args), inner)
		{
		}
	}
}
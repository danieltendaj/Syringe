using System;

namespace Syringe.Core.Exceptions
{
    public class InvalidEngineException : Exception
    {
        public InvalidEngineException(string message) : base(message)
		{ }

        public InvalidEngineException(string message, params object[] args) : base(string.Format(message, args))
		{ }
    }
}
using System;
using System.Text;

namespace Syringe.Core.Logging
{
    public class SimpleLogger
    {
        private readonly StringBuilder _stringBuilder;

        public SimpleLogger()
        {
            _stringBuilder = new StringBuilder();
        }

        public string GetLog()
        {
            return _stringBuilder.ToString();
        }

        public void Write(string message, params object[] args)
        {
	        if (!string.IsNullOrEmpty(message))
	        {
		        message = (args.Length > 0) ? string.Format(message, args) : message;
				_stringBuilder.Append(message);
			}
		}

        public void WriteLine(string message, params object[] args)
        {
            WriteLine(null, message, args);
        }

        public void WriteLine(Exception ex, string message, params object[] args)
        {
            if (ex != null)
                message += "\n" + ex;

	        if (!string.IsNullOrEmpty(message))
	        {
		        Write(message, args);
		        Write(System.Environment.NewLine);
	        }
        }
    }
}

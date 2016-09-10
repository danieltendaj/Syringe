using System;
using System.Text;

namespace Syringe.Core.Logging
{
    public class SimpleLogger
    {
        internal readonly StringBuilder LogStringBuilder;

        public SimpleLogger()
        {
            LogStringBuilder = new StringBuilder();
        }

        public string GetLog()
        {
            return LogStringBuilder.ToString();
        }

        public void AppendTextLine(string text)
        {
            LogStringBuilder.AppendLine(text);
        }

        public void Write(string message, params object[] args)
        {
	        if (!string.IsNullOrEmpty(message))
	        {
                string fullMessage = FormatMessage(message, args);
                LogStringBuilder.Append(fullMessage);
            }
		}

        public void WriteLine(string message, params object[] args)
        {
            WriteLine(null, message, args);
        }

		public void WriteLine(Exception ex, string message, params object[] args)
        {
            if (ex != null)
                message += System.Environment.NewLine + ex;

	        if (!string.IsNullOrEmpty(message))
	        {
                string fullMessage = FormatMessage(message, args);
                LogStringBuilder.AppendLine(fullMessage);
	        }
        }

        private string FormatMessage(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    message = string.Format(message, args);
                }
                catch (FormatException)
                {
                    message = $"Logger caught a formatting exception. Message: '{message}' args.length: {args.Length}";
                }
            }

            string now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string fullMessage = $"{now}  |  {message}";
            return fullMessage;
        }
    }
}
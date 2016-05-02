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
            WriteLine(null, 0, message, args);
        }

		public void WriteIndentedLine(string message, params object[] args)
		{
			WriteLine(null, 1, message, args);
		}

		public void WriteDoubleIndentedLine(string message, params object[] args)
		{
			WriteLine(null, 2, message, args);
		}

		public void WriteLine(Exception ex, int indents, string message, params object[] args)
        {
            if (ex != null)
                message += "\n" + ex;

	        if (!string.IsNullOrEmpty(message))
	        {
		        for (int i = 0; i < indents; i++)
		        {
			        Write("  ");
		        }

		        Write("=> "+message, args);
		        Write(System.Environment.NewLine);
	        }
        }
    }
}

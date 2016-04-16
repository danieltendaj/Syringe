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
            _stringBuilder.Append(args.Length > 0 ? string.Format(message, args) : message);
        }

        public void WriteLine(string message, params object[] args)
        {
            WriteLine(null, message, args);
        }

        public void WriteLine(Exception ex, string message, params object[] args)
        {
            if (ex != null)
                message += "\n" + ex;

            Write(message, args);
        }
    }
}

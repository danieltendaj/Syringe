using System;
using System.IO;
using System.Reflection;

namespace Syringe.Tests
{
	public class TestHelpers
	{
		public static string ReadEmbeddedFile(string file, string namespacePath)
		{
			string resourcePath = string.Format("{0}{1}", namespacePath, file);

			Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourcePath);
			if (stream == null)
				throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", resourcePath));

			string textContent = "";
			using (StreamReader reader = new StreamReader(stream))
			{
				textContent = reader.ReadToEnd();
			}

			return textContent;
		}
	}
}
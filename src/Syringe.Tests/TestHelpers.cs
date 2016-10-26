using System;
using System.IO;
using System.Reflection;

namespace Syringe.Tests
{
	public class TestHelpers
	{
		public static string ReadEmbeddedFile(string file, string namespacePath)
		{
			string resourcePath = $"{namespacePath}{file}";

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
		    if (stream == null)
		    {
		        throw new InvalidOperationException($"Unable to find '{resourcePath}' as an embedded resource");
		    }

			string textContent;
			using (var reader = new StreamReader(stream))
			{
				textContent = reader.ReadToEnd();
			}

			return textContent;
		}
	}
}
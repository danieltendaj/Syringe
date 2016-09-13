using Syringe.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Tests.Scripting
{
	public interface ISnippetFileReader
	{
		string ReadFile(string path);
		IEnumerable<string> GetSnippetFilenames(ScriptSnippetType snippetType);
	}
}

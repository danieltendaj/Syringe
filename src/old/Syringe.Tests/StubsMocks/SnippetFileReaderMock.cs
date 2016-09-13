using System.Collections.Generic;
using Syringe.Core.Tests.Scripting;

namespace Syringe.Tests.StubsMocks
{
    public class SnippetFileReaderMock : ISnippetFileReader
    {
        private string _content;

        public SnippetFileReaderMock(string content)
        {
            _content = content;
        }

        public string ReadFile(string path)
        {
            return _content;
        }

        public IEnumerable<string> GetSnippetFilenames(ScriptSnippetType snippetType)
        {
            return new string[] {"file1", "file2"};
        }
    }
}
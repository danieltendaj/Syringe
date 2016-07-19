namespace Syringe.Core.Tests.Scripting
{
	/// <summary>
	/// Contains information about all snippet files for a test,
	/// currently just snippets that execute before a test runs.
	/// </summary>
    public class ScriptSnippets
    {
        public string BeforeExecuteFilename { get; set; }
    }
}
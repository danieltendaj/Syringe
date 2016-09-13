using System;
using System.Diagnostics;

namespace Syringe.Core.Tests.Variables
{
    [DebuggerDisplay("Name = {Name} | Value = {Value} | Environment = {Environment.Name}")]
    public class Variable : IVariable
    {
		public string Name { get; set; }
		public string Value { get; set; }
		public Environment.Environment Environment { get; set; }

		public Variable() { }

		public Variable(string name, string value, string environment)
		{
			Name = name;
			Value = value;
			Environment = new Environment.Environment { Name = environment };
		}

	    public bool MatchesEnvironment(string environmentToTest)
	    {
	        environmentToTest = environmentToTest ?? string.Empty;
	        string thisEnvironment = Environment?.Name ?? string.Empty;

	        bool matched = string.IsNullOrEmpty(thisEnvironment) || environmentToTest.Equals(thisEnvironment, StringComparison.InvariantCultureIgnoreCase);
	        return matched;
	    }

	    public bool MatchesNameAndEnvironment(Variable variableToTest)
	    {
	        bool matched = false;

	        if (variableToTest != null)
	        {
	            matched = Name.Equals(variableToTest.Name, StringComparison.InvariantCultureIgnoreCase);
	            if (matched)
	            {
                    matched = Environment.Name.Equals(variableToTest.Environment.Name, StringComparison.InvariantCultureIgnoreCase);
                }
            }

	        return matched;
	    }
	}
}
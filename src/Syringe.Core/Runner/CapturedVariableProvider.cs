using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner
{
	internal class CapturedVariableProvider
	{
	    private readonly string _environment;
	    private readonly List<Variable> _currentVariables;

		public CapturedVariableProvider(string environment)
		{
		    _environment = environment;
		    _currentVariables = new List<Variable>();
		}

	    public void AddOrUpdateVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				AddOrUpdateVariable(variable);
			}
		}

		public void AddOrUpdateVariable(Variable variable)
		{
		    bool shouldAddOrUpdate = variable.MatchesEnvironment(_environment);

		    if (shouldAddOrUpdate)
		    {
		        Variable detectedVariable = _currentVariables.FirstOrDefault(x => x.Name.Equals(variable.Name, StringComparison.InvariantCultureIgnoreCase));
		        if (detectedVariable != null)
		        {
		            detectedVariable.Value = variable.Value;
		        }
		        else
		        {
		            _currentVariables.Add(variable);
		        }
		    }
		}

		public string GetVariableValue(string name)
		{
			var variable = _currentVariables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			if (variable != null)
				return variable.Value;

			return "";
		}

		public string ReplacePlainTextVariablesIn(string text)
		{
			foreach (Variable variable in _currentVariables)
			{
				text = text.Replace("{" + variable.Name + "}", variable.Value);
			}

			return text;
		}

		public string ReplaceVariablesIn(string text)
		{
			foreach (Variable variable in _currentVariables)
			{
				text = text.Replace("{" + variable.Name + "}", Regex.Escape(variable.Value));
			}

			return text;
		}

		public void Dump()
		{
			Log.Information("In my bag of magic variables I have:");

			foreach (Variable variable in _currentVariables)
			{
				Log.Information(" - {{{0}}} : {1}", variable.Name, variable.Value);
			}
		}

		/// <summary>
		/// Finds text in the content, returning them as variables, e.g. {capturedvariable1} = value
		/// </summary>
		public static List<Variable> MatchVariables(List<CapturedVariable> capturedVariables, string content, SimpleLogger simpleLogger)
		{
			var variables = new List<Variable>();

			foreach (CapturedVariable regexItem in capturedVariables)
			{
				simpleLogger.WriteLine("");
				simpleLogger.WriteLine("---------------------------");
				simpleLogger.WriteLine("Testing {{capturedvariable{0}}}", regexItem.Name);
				simpleLogger.WriteLine(" - Regex: {0}", regexItem.Regex);

				string capturedValue = "";
				try
				{
					var regex = new Regex(regexItem.Regex, RegexOptions.IgnoreCase);
					if (regex.IsMatch(content))
					{
						MatchCollection matches = regex.Matches(content);
						foreach (Match match in matches)
						{
							if (match.Groups.Count > 1)
							{
								capturedValue += match.Groups[1];
								Log.Information(" - Found: {0}", capturedValue);
							}
						}
					}
					else
					{
						simpleLogger.WriteLine(" - No match");
					}
				}
				catch (ArgumentException e)
				{
					simpleLogger.WriteLine(" - Invalid regex: {0}", e.Message);
				}

				variables.Add(new Variable("capturedvariable" + regexItem.Name, capturedValue, ""));
			}

			return variables;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Runner
{
    public class CapturedVariableProvider : ICapturedVariableProvider
    {
        private readonly IVariableContainer _currentVariables;
        private readonly string _environment;

        public CapturedVariableProvider(IVariableContainer variableContainer, string environment)
        {
            _currentVariables = variableContainer;
            _environment = environment;
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
                    bool shouldUpdateValue = string.IsNullOrEmpty(detectedVariable.Environment.Name) || !string.IsNullOrEmpty(variable.Environment.Name);

                    if (shouldUpdateValue)
                    {
                        detectedVariable.Value = variable.Value;
                    }
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
            return variable != null ? variable.Value : string.Empty;
        }

        public string ReplacePlainTextVariablesIn(string text)
        {
            text = text ?? string.Empty;

            foreach (Variable variable in _currentVariables)
            {
                if (variable.MatchesEnvironment(_environment))
                {
                    text = text.Replace("{" + variable.Name + "}", variable.Value);
                }
            }

            return text;
        }

        public string ReplaceVariablesIn(string text)
        {
            string result = text ?? string.Empty;

            foreach (Variable variable in _currentVariables)
            {
                if (variable.MatchesEnvironment(_environment))
                {
                    result = result.Replace("{" + variable.Name + "}", Regex.Escape(variable.Value));
                }
            }

            return result;
        }

        /// <summary>
        /// Finds text in the content, returning them as variables, e.g. {capturedvariable1} = value
        /// </summary>
        public static List<Variable> MatchVariables(List<CapturedVariable> capturedVariables, string content, SimpleLogger simpleLogger)
        {
            var variables = new List<Variable>();
            var variablePostProcessor = new VariablePostProcessor();

            foreach (CapturedVariable regexItem in capturedVariables)
            {
                simpleLogger.WriteLine("Parsing captured variable '{{{0}}}'", regexItem.Name);
                simpleLogger.WriteIndentedLine("Regex: {0}", regexItem.Regex);

                string capturedValue = "";
                try
                {
                    var regex = new Regex(regexItem.Regex, RegexOptions.IgnoreCase);
                    if (regex.IsMatch(content))
                    {
                        MatchCollection matches = regex.Matches(content);
                        int matchCount = 0;
                        foreach (Match match in matches)
                        {
                            if (match.Groups.Count > 1)
                            {
                                string detectedValue = match.Groups[1].Value;
                                simpleLogger.WriteIndentedLine($"Detected value: {detectedValue}");

                                string transformedValue = variablePostProcessor.Process(detectedValue, regexItem.PostProcessorType);
                                simpleLogger.WriteIndentedLine($"Transformed value: {detectedValue}");

                                capturedValue = transformedValue;
                                simpleLogger.WriteIndentedLine($"{++matchCount}. '{regexItem.Regex}' matched, updated variable to '{capturedValue}'");
                                break;
                            }

                            simpleLogger.WriteIndentedLine("{0}. '{1}' matched, but the regex has no capture groups so the variable value wasn't updated.", ++matchCount, regexItem.Regex);
                        }
                    }
                    else
                    {
                        simpleLogger.WriteIndentedLine("No match");
                    }
                }
                catch (ArgumentException e)
                {
                    simpleLogger.WriteIndentedLine("Invalid regex: {0}", e.Message);
                }

                variables.Add(new Variable(regexItem.Name, capturedValue, ""));
            }

            return variables;
        }
    }
}
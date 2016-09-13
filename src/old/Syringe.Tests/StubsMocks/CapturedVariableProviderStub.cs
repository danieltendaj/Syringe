using System.Collections.Generic;
using Syringe.Core.Runner;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.StubsMocks
{
    public class CapturedVariableProviderStub : ICapturedVariableProvider
    {
        public List<Variable> Variables = new List<Variable>(); 

        public void AddOrUpdateVariable(Variable variable)
        {
            Variables.Add(variable);
        }

        public void AddOrUpdateVariables(List<Variable> variables)
        {
            Variables.AddRange(variables);
        }

        public string GetVariableValue_Value { get; set; }
        public string GetVariableValue(string name)
        {
            return GetVariableValue_Value;
        }
        
        public string ReplacePlainTextVariablesIn(string text)
        {
            return text;
        }
        
        public string ReplaceVariablesIn(string text)
        {
            return text;
        }
    }
}
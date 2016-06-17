using System.Collections.Generic;
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Runner
{
    public interface ICapturedVariableProvider
    {
        void AddOrUpdateVariable(Variable variable);
        void AddOrUpdateVariables(List<Variable> variables);
        string GetVariableValue(string name);
        string ReplacePlainTextVariablesIn(string text);
        string ReplaceVariablesIn(string text);
    }
}
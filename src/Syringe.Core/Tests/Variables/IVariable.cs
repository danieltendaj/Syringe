using Syringe.Core.Environment;

namespace Syringe.Core.Tests.Variables
{
    public interface IVariable
    {
        Environment.Environment Environment { get; set; }
        string Name { get; set; }
        string Value { get; set; }

        bool MatchesEnvironment(string environmentToTest);
        bool MatchesNameAndEnvironment(Variable variableToTest);
    }
}
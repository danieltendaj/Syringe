namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class EnvironmentVariable : IReservedVariable
    {
        private readonly string _environment;
        public string Description => "Returns the current environment that test is run under.";
        public string Name => "_environment";

        public EnvironmentVariable(string environment)
        {
            _environment = environment;
        }

        public Variable CreateVariable()
        {
            return new Variable(Name, _environment, string.Empty);
        }
    }
}
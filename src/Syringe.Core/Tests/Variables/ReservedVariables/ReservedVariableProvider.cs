using System;

namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class ReservedVariableProvider : IReservedVariableProvider
    {
        internal readonly string Environment;
        private readonly DateTime _createdDate = DateTime.Now;

        public ReservedVariableProvider(string environment)
        {
            Environment = environment;
        }

        public IReservedVariable[] ListAvailableVariables()
        {
            return new IReservedVariable[]
            {
                new RandomNumberVariable(),
                new TestRunVariable(_createdDate),
                new EnvironmentVariable(Environment), 
            };
        }
    }
}
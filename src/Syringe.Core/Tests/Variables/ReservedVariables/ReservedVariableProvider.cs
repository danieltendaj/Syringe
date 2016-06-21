using System;

namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class ReservedVariableProvider : IReservedVariableProvider
    {
        private readonly string _environment;
        private readonly DateTime _createdDate = DateTime.Now;

        public ReservedVariableProvider(string environment)
        {
            _environment = environment;
        }

        public IReservedVariable[] ListAvailableVariables()
        {
            return new IReservedVariable[]
            {
                new RandomNumberVariable(),
                new TestRunVariable(_createdDate),
                new EnvironmentVariable(_environment), 
            };
        }
    }
}
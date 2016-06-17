using System;

namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class ReservedVariableProvider : IReservedVariableProvider
    {
        private readonly DateTime _createdDate = DateTime.Now;

        public IReservedVariable[] ListAvailableVariables()
        {
            return new IReservedVariable[]
            {
                new RandomNumberVariable(),
                new TestRunVariable(_createdDate), 
            };
        }
    }
}
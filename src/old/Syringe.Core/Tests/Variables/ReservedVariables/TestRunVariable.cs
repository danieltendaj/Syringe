using System;

namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class TestRunVariable : IReservedVariable
    {
        private DateTime _createdDate;

        public TestRunVariable(DateTime createdDate)
        {
            _createdDate = createdDate;
        }

        public string Description => "Returns the time the test started";
        public string Name => "_testRunTimestamp";

        public IVariable CreateVariable()
        {
            return new Variable(Name, _createdDate.ToString("s"), "");
        }
    }
}
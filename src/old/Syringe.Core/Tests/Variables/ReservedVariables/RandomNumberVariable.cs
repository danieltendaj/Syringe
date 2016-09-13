using System;

namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class RandomNumberVariable : IReservedVariable
    {
        public string Description => "Returns a random number each time it is used.";
        public string Name => "_randomNumber";
        private readonly Random _random = new Random();

        public IVariable CreateVariable()
        {
            string randomNumber = _random.Next().ToString();
            return new Variable(Name, randomNumber, string.Empty);
        }
    }
}
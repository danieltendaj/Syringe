namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class RandomNumberVariable : IReservedVariable
    {
        public string Description => "Returns a random number each time it is used.";
        public string Name => "_randomNumber";

        public Variable CreateVariable()
        {
            throw new System.NotImplementedException();
        }
    }
}
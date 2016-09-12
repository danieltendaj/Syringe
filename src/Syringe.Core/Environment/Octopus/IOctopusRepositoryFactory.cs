using Octopus.Client;

namespace Syringe.Core.Environment.Octopus
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository Create();
    }
}
using Octopus.Client;

namespace Syringe.Core.Environment
{
    public interface IOctopusRepositoryFactory
    {
        IOctopusRepository Create();
    }
}
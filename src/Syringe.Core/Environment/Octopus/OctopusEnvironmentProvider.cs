using System.Collections.Generic;
using System.Linq;
using Octopus.Client;

namespace Syringe.Core.Environment.Octopus
{
    public class OctopusEnvironmentProvider : IEnvironmentProvider
    {
        private readonly IOctopusRepository _repository;
        private Environment[] _environments = null;

        public OctopusEnvironmentProvider(IOctopusRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Environment> GetAll()
        {
            if (_environments == null)
            {
                _environments = _repository.Environments
                    .FindAll()
                    .Select(x => new Environment
                    {
                        Name = x.Name,
                        Order = x.SortOrder
                    })
                    .ToArray();
            }

            return _environments;
        }
    }
}
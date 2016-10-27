using System.Collections.Generic;
using System.Linq;

namespace Syringe.Core.Environment.Octopus
{
    public class OctopusEnvironmentProvider : IEnvironmentProvider
    {
        //private Environment[] _environments = null;

        public IEnumerable<Environment> GetAll()
        {
            throw new System.NotImplementedException();
        }

        //private readonly IOctopusRepository _repository;
        //public OctopusEnvironmentProvider(IOctopusRepository repository)
        //{
        //    _repository = repository;
        //}

        //public IEnumerable<Environment> GetAll()
        //{
        //    if (_environments == null)
        //    {
        //        _environments = _repository.Environments
        //            .FindAll()
        //            .Select(x => new Environment
        //            {
        //                Name = x.Name,
        //                Order = x.SortOrder
        //            })
        //            .ToArray();
        //    }

        //    return _environments;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Syringe.Core.Environment
{
    public class OctopusEnvironmentProvider : IEnvironmentProvider
    {
        private readonly IOctopusRepository _repository;

        public OctopusEnvironmentProvider(IOctopusRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Environment> GetAll()
        {
            return _repository.Environments
                              .FindAll()
                              .Select(x => new Environment
                              {
                                  Name = x.Name,
                                  Order = x.SortOrder,
                                  Roles = _repository.Environments
                                      .GetMachines(x)
                                      .Select(m => new EnvironmentRole
                                      {
                                          Name = m.Name,
                                          HostName = ParseEndpoint(m.Endpoint as ListeningTentacleEndpointResource),
                                          Role = ParseRoles(m.Roles)
                                      })
                              });            
        }

        private static string ParseRoles(ReferenceCollection roles)
        {
            return string.Join(",", roles.Select(r => r.ToString()).ToArray());
        }

        private static string ParseEndpoint(ListeningTentacleEndpointResource endpoint)
        {
            return endpoint == null
                ? string.Empty
                : new Uri(endpoint.Uri).Host;
        }
    }
}
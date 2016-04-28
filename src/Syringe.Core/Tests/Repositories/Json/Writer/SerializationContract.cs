using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syringe.Core.Tests.Repositories.Json.Writer
{
    public class SerializationContract : DefaultContractResolver
    {
        private readonly string[] _propertiesToIgnore = 
        {
            "Filename"
        };

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> defaultList = base.CreateProperties(type, memberSerialization).ToList();

            List<JsonProperty> filtered = defaultList
                            .Where(x => !_propertiesToIgnore.Any(p => p.Equals(x.PropertyName, StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();

            return filtered;
        }

        public static JsonSerializerSettings GetSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SerializationContract()
            };

            return settings;
        }
    }
}
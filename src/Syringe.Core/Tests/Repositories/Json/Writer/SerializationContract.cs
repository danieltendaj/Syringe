using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syringe.Core.Tests.Repositories.Json.Writer
{
    public class SerializationContract : DefaultContractResolver
    {
        private readonly string[] _propertiesToIgnore =
        {
            "Filename",
            "TransformedValue"
        };

        private readonly PropertyInfo[] _propsToIgnore =
        {
            typeof (Test).GetProperty("Filename"),
            typeof (Test).GetProperty("Position"),
            typeof (Test).GetProperty("AvailableVariables"),
            typeof (TestFile).GetProperty("Filename"),
            typeof (TestFile).GetProperty("Environment"),
            typeof (Variable).GetProperty("Order"),
            typeof (Variable).GetProperty("Roles"),
        };

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> defaultList = base.CreateProperties(type, memberSerialization).ToList();

            List<JsonProperty> filtered = defaultList
                                .Where(x => !_propsToIgnore.Any(p => GetFullName(p) == GetFullName(x)))
                                .ToList();

            //List<JsonProperty> filtered = defaultList
            //                .Where(x => !_propertiesToIgnore.Any(p => p.Equals(x.PropertyName, StringComparison.InvariantCultureIgnoreCase)))
            //                .ToList();

            return filtered;
        }

        private string GetFullName(PropertyInfo propertyInfo)
        {
            return $"{propertyInfo?.DeclaringType?.FullName}.{propertyInfo?.Name}";
        }

        private string GetFullName(JsonProperty jsonProperty)
        {
            return $"{jsonProperty?.DeclaringType?.FullName}.{jsonProperty?.PropertyName}";
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
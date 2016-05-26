using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syringe.Core.Tests.Repositories.Json.Writer
{
    public class SerializationContract : DefaultContractResolver
    {
        private readonly string[] _propertiesToIgnore =
        {
            GetPropertyFullName<Test>(x => x.AvailableVariables),
            GetPropertyFullName<TestFile>(x => x.Filename),
            GetPropertyFullName<Environment.Environment>(x => x.Order),
            GetPropertyFullName<Environment.Environment>(x => x.Roles),
            GetPropertyFullName<Assertion>(x => x.TransformedValue),
            GetPropertyFullName<Assertion>(x => x.Log),
            GetPropertyFullName<Assertion>(x => x.Success),
        };
        
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> defaultList = base.CreateProperties(type, memberSerialization).ToList();
            List<JsonProperty> filtered = defaultList
                                .Where(x => !_propertiesToIgnore.Any(p => p == GetFullName(x)))
                                .ToList();

            return filtered;
        }
        
        private static string GetFullName(JsonProperty jsonProperty)
        {
            return $"{jsonProperty?.DeclaringType?.FullName}.{jsonProperty?.PropertyName}";
        }

        /// <summary>
        /// http://stackoverflow.com/a/14187873
        /// </summary>
        private static string GetPropertyFullName<T>(Expression<Func<T, object>> prop)
        {
            MemberExpression expr;

            var body = prop.Body as MemberExpression;
            if (body != null)
            {
                expr = body;
            }
            else
            {
                expr = (MemberExpression)((UnaryExpression)prop.Body).Operand;
            }

            string name = $"{expr.Member.DeclaringType?.FullName}.{expr.Member.Name}";
            return name;
        }
    }
}
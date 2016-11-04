using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Syringe.Core.Tests
{
    [BsonIgnoreExtraElements]
    public class TestConditions
    {
        public List<string> RequiredEnvironments { get; set; } = new List<string>();
    }
}
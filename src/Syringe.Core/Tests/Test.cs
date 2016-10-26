using System;
using System.Collections.Generic;
using System.Net;
using MongoDB.Bson.Serialization.Attributes;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Tests
{
    [BsonIgnoreExtraElements]
    public class Test
	{
		public string Description { get; set; }
        public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }
	    public HttpStatusCode ExpectedHttpStatusCode { get; set; }
		public List<HeaderItem> Headers { get; set; }

		public List<CapturedVariable> CapturedVariables { get; set; }
		public List<Assertion> Assertions { get; set; }
        public List<Variable> AvailableVariables { get; set; }

        public ScriptSnippets ScriptSnippets { get; set; }

	    public TestConditions TestConditions { get; set; }

	    public Test()
		{
			Headers = new List<HeaderItem>();
			CapturedVariables = new List<CapturedVariable>();
			Assertions = new List<Assertion>();
            AvailableVariables = new List<Variable>();
            ScriptSnippets = new ScriptSnippets();
            TestConditions = new TestConditions();

        }

		public void AddHeader(string key, string value)
		{
			Headers.Add(new HeaderItem(key, value));
        }
    }
}
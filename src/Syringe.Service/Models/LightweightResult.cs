using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Models
{
	public class LightweightResult
	{
		public string TestDescription { get; set; }
		public string TestUrl { get; set; }
		public string ActualUrl { get; set; }
		public string Message { get; set; }
		public TimeSpan ResponseTime { get; set; }
		public bool ResponseCodeSuccess { get; set; }
		public string ExceptionMessage { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TestResultState ResultState { get; set; }
        public bool AssertionsSuccess { get; set; }
		public bool ScriptCompilationSuccess { get; set; }
	}
}
using System.Collections.Generic;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Tasks
{
	public class TaskDetails
	{
		public int TaskId { get; set; }
		public string Filename { get; set; }
		public string Username { get; set; }

		public string Status { get; set; }
		public bool IsComplete { get; set; }
		public int CurrentIndex { get; set; }
		public int TotalTests { get; set; }

		public List<TestResult> Results { get; set; }
		public string Errors { get; set; }

		public TaskDetails()
		{
			Results = new List<TestResult>();
		}
	}
}
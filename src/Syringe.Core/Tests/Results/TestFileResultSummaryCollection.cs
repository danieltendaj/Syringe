using System;
using System.Collections.Generic;

namespace Syringe.Core.Tests.Results
{
	public class TestFileResultSummaryCollection
	{
		public long TotalFileResults { get; set; }
        public IEnumerable<TestFileResultSummary> PagedResults { get; set; }
    }
}
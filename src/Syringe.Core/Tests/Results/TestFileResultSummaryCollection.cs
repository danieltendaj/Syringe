using System.Collections.Generic;

namespace Syringe.Core.Tests.Results
{
	public class TestFileResultSummaryCollection
	{
		public long TotalFileResults { get; set; }
		public int PageNumber { get; set; }
        public IEnumerable<TestFileResultSummary> PagedResults { get; set; }
	    public int NoOfResults { get; set; }
	    public double PageNumbers { get; set; }
	    public IEnumerable<string> Environments { get; set; }
	}
}
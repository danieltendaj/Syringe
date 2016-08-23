using System.Collections.Generic;

namespace Syringe.Service.Models
{
    public class BatchStatus
    {
        public int BatchId { get; set; }
        public List<TestFileRunResult> TestFilesResult { get; set; }
        public bool Completed { get; set; }
        public bool AllTestsPassed { get; set; }
    }
}
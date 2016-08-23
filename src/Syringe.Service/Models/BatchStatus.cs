using System.Collections.Generic;

namespace Syringe.Service.Models
{
    public class BatchStatus
    {
        public int BatchId { get; set; }
        public List<TestFileRunResult> TestFilesResult { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Syringe.Service.Models
{
    public class BatchStatus
    {
        public int BatchId { get; set; }
        public bool BatchFinished { get; set; }
        public bool HasFailedTests { get; set; }
        public int TestFilesRunning { get; set; }
        public int TestFilesFinished { get; set; }
        public int TestFilesFailed { get; set; }
        public IEnumerable<Guid> TestFilesResultIds { get; set; }
        public IEnumerable<Guid> TestFilesWithFailedTests { get; set; } 
        public IEnumerable<int> FailedTasks { get; set; } 
    }
}
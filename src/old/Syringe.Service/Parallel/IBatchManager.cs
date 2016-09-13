using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public interface IBatchManager
    {
        int StartBatch(string[] filenames, string environment, string username);
        BatchStatus GetBatchStatus(int batchId);
    }
}
using System.Collections.Generic;
using Syringe.Core.Tasks;

namespace Syringe.Core.Services
{
    public interface ITasksService
    {
        int Start(TaskRequest item);
        IEnumerable<TaskDetails> GetTasks();
        TaskDetails GetTask(int taskId);
        int StartBatch(string[] fileNames, string environment, string username);
    }
}
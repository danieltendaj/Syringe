using System.Globalization;

namespace Syringe.Service.Controllers.Hubs
{
    internal class TaskGroupProvider : ITaskGroupProvider
    {
        public string GetGroupForTask(int taskId)
        {
            return string.Format(CultureInfo.InvariantCulture, "TaskGroup-{0}", taskId);
        }
    }
}
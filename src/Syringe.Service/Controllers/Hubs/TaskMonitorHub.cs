using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Syringe.Service.Parallel;

namespace Syringe.Service.Controllers.Hubs
{
	public class TaskMonitorHub : Hub<ITaskMonitorHubClient>
	{
		private readonly ITaskGroupProvider _taskGroupProvider;
		private readonly ITaskObserver _taskObserver;

		public TaskMonitorHub(ITaskGroupProvider taskGroupProvider, ITaskObserver taskObserver)
		{
			_taskGroupProvider = taskGroupProvider;
			_taskObserver = taskObserver;
		}

		public async Task<TaskState> StartMonitoringTask(int taskId)
		{
			await Groups.Add(Context.ConnectionId, _taskGroupProvider.GetGroupForTask(taskId));

			TaskMonitoringInfo details = _taskObserver.StartMonitoringTask(taskId);

			if (details == null)
			{
				return new TaskState { TotalTests = 0 };
			}

			return new TaskState { TotalTests = details.TotalTests };
		}
	}
}

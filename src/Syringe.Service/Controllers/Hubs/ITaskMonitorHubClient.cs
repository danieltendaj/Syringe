namespace Syringe.Service.Controllers.Hubs
{
	public interface ITaskMonitorHubClient
	{
		void OnTaskCompleted(CompletedTaskInfo taskInfo);
	    void OnTestFileGuid(string guid);
	}
}
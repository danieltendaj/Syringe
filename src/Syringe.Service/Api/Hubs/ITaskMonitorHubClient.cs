namespace Syringe.Service.Api.Hubs
{
	public interface ITaskMonitorHubClient
	{
		void OnTaskCompleted(CompletedTaskInfo taskInfo);
	    void OnTestFileGuid(string guid);
	}
}
namespace Syringe.Service.Parallel
{
    public interface ITaskObserver
    {
        TaskMonitoringInfo StartMonitoringTask(int taskId);
    }
}
namespace Syringe.Service.Controllers.Hubs
{
    public interface ITaskGroupProvider
    {
        string GetGroupForTask(int taskId);
    }
}
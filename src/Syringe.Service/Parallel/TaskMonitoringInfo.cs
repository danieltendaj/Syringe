namespace Syringe.Service.Parallel
{
    public class TaskMonitoringInfo
    {
        public TaskMonitoringInfo(int totalTests)
        {
            TotalTests = totalTests;
        }

        public int TotalTests { get; private set; }
    }
}
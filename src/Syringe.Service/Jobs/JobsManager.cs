namespace Syringe.Service.Jobs
{
    public class JobsManager : IJob
    {
        private readonly IJob[] _jobs;

        public JobsManager(DbCleanupJob dbCleanupJob)
        {
            _jobs = new IJob[] { dbCleanupJob };
        }

        public void Start()
        {
            foreach (IJob job in _jobs)
            {
                job.Start();
            }
        }

        public void Stop()
        {
            foreach (IJob job in _jobs)
            {
                job.Stop();
            }
        }
    }
}
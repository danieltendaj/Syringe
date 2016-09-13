namespace Syringe.Service.Jobs
{
    public interface IDbCleanupJob
    {
        void Start();
        void Stop();
    }
}
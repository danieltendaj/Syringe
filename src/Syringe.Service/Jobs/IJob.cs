namespace Syringe.Service.Jobs
{
    public interface IJob
    {
        void Start();
        void Stop();
    }
}
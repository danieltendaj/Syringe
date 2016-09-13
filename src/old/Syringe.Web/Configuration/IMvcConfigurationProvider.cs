namespace Syringe.Web.Configuration
{
    public interface IMvcConfigurationProvider
    {
        MvcConfiguration Load();
    }
}
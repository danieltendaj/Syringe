namespace Syringe.Web.Models
{
    public class EnvironmentSelectorViewModel
    {
        public string[] Environment { get; set; }
        public RunButtonType RunButtonType { get; set; }

        public EnvironmentSelectorViewModel(string[] environment, RunButtonType runButtonType)
        {
            Environment = environment;
            RunButtonType = runButtonType;
        }
    }

    public enum RunButtonType
    {
        PlayButton,
        RunAllTests
    }
}
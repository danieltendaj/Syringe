namespace Syringe.Core.Tests.Variables
{
	public class CapturedVariable
	{
		public string Name { get; set; }
		public string Regex { get; set; }
	    public VariablePostProcessorType PostProcessorType { get; set; }

		public CapturedVariable() : this("","")
		{ }

	    public CapturedVariable(string name, string regex) : this(name, regex, VariablePostProcessorType.None)
	    { }

	    public CapturedVariable(string name, string regex, VariablePostProcessorType postProcessorType)
        {
            Name = name;
			Regex = regex;
            PostProcessorType = postProcessorType;
		}
	}
}
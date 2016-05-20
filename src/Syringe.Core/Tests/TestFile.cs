using System.Collections.Generic;
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Tests
{
	public class TestFile
	{
		public IEnumerable<Test> Tests { get; set; }
		public string Filename { get; set; }
		public List<Variable> Variables { get; set; }
	    public string Environment { get; set; }

	    public TestFile()
		{
			Variables = new List<Variable>();
			Tests = new List<Test>();
		}
	}
}
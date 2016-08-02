
namespace Syringe.Core.Tests
{
	public class Assertion
	{
		public string Description { get; set; }
		public AssertionMethod AssertionMethod { get; set; }
		public string Value { get; set; }
		public string TransformedValue { get; set; }
		public bool Success { get; set; }
		public AssertionType AssertionType { get; set; }
		public string Log { get; set; }

		public Assertion() : this("","", AssertionType.Positive, AssertionMethod.Regex)
		{
		}

		public Assertion(string description, string value, AssertionType assertionType, AssertionMethod assertionMethod)
		{
			Description = description;
			Value = value;
			AssertionType = assertionType;
			AssertionMethod = assertionMethod;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Description, TransformedValue);
		}
	}
}
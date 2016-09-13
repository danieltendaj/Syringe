namespace Syringe.Core.Tests
{
	public class HeaderItem
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public HeaderItem() : this("", "")
		{
		}

		public HeaderItem(string key, string value)
		{
			Key = key;
			Value = value;
		}
	}
}

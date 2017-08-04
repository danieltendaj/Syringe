namespace Syringe.Tests.NUnitToXUnit
{
	public class Contains
	{
		public object TheItem { get; set; }
		public bool IsInverse { get; set; }

		public static Contains Item(object item)
		{
			return new Contains()
			{
				TheItem = item
			};
		}
	}
}
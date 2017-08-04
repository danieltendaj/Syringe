namespace Syringe.Tests.NUnitToXUnit
{
	public class Is
	{
		public object Item { get; set; }

		public static Not Not
		{
			get
			{
				return new Not() { IsNot = true };
			}
		}

		public static object Null => null;

		public override bool Equals(object obj)
		{
			return obj.Equals(Item);
		}

		public static Is EqualTo(object item)
		{
			return new Is() { Item = item };
		}
	}
}
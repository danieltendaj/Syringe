using System.Collections.Generic;

namespace Syringe.Tests.NUnitToXUnit
{
	public class Not : Is, IEqualityComparer<Not>
	{
		public bool IsNot { get; set; }
		public bool IsNotNull { get; set; }

		public Not Null
		{
			get
			{
				return new Not()
				{
					IsNotNull = true
				};
			}
		}

		public override bool Equals(object obj)
		{
			if (IsNotNull)
			{
				return obj != null;
			}

			if (IsNot)
				return !obj.Equals(Item);

			return obj.Equals(Item);
		}

		public Is EqualTo(object item)
		{
			return new Is() { Item = item };
		}

		public Contains Contains(object item)
		{
			return new Contains()
			{
				TheItem = item,
				IsInverse = true
			};
		}

		public bool Equals(Not x, Not y)
		{
			return Equals(y);
		}

		public int GetHashCode(Not obj)
		{
			return base.GetHashCode();
		}
	}
}
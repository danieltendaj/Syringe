using System;
using System.Collections.Generic;
using Xunit;

namespace Syringe.Tests.NUnitToXUnit
{
	public class NUnitAssert
	{
		public static void Fail(string message = "")
		{
			throw new Exception(message);
		}

		public static void That(object a, object b, string message = "", params object[] args)
		{
			Assert.Equal(a, b);
		}

		public static void That(object a, Is isItem, string message = "", params object[] args)
		{
			Assert.Equal(a, isItem);
		}

		public static void That(IEnumerable<object> a, Contains containsItem, string message = "", params object[] args)
		{
			Assert.Contains(containsItem.TheItem, a);
		}
	}
}
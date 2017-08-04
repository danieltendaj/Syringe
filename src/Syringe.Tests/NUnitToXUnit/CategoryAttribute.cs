using System;
using Xunit.Sdk;

namespace Syringe.Tests.NUnitToXUnit
{
	[TraitDiscoverer(CategoryDiscoverer.DiscovererTypeName, DiscovererUtil.AssemblyName)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class CategoryAttribute : Attribute, ITraitAttribute
	{
		public CategoryAttribute(string categoryName)
		{
			this.Name = categoryName;
		}

		public string Name { get; private set; }
	}
}
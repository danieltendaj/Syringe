using System;
using System.Collections.Generic;

namespace Syringe.Core.Configuration
{
	public interface IConfiguration
	{
		Settings Settings { get; set; }
		IEnumerable<Environment.Environment> Environments { get; set; }
	}
}
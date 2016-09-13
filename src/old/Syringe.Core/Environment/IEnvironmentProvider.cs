using System.Collections.Generic;

namespace Syringe.Core.Environment
{
	public interface IEnvironmentProvider
	{
		IEnumerable<Environment> GetAll();
	}
}
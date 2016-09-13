using System.Collections.Generic;

namespace Syringe.Core.Services
{
	public interface IEnvironmentsService
	{
		IEnumerable<Environment.Environment> Get();
	}
}
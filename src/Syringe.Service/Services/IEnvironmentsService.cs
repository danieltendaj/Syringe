using System.Collections.Generic;
using Syringe.Core.Environment;

namespace Syringe.Service.Services
{
	public interface IEnvironmentsService
	{
		IEnumerable<Environment> List();
	}
}
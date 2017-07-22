using System.Collections.Generic;
using Syringe.Core.Security;

namespace Syringe.Web.Models
{
	public interface IRunViewModel
	{
		void Run(IUserContext userContext, string fileName, string environment);

		IEnumerable<RunningTestViewModel> Tests { get; }
		int CurrentTaskId { get; }
		string FileName { get; }
		string Environment { get; }
	}
}
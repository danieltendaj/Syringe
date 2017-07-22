using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Syringe.Web.Models
{
	public class VariableViewModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Value { get; set; }

		[Required]
		public string Environment { get; set; }

		public SelectListItem[] AvailableEnvironments { get; set; }
	}
}
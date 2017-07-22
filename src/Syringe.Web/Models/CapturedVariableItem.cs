using System.ComponentModel.DataAnnotations;
using Syringe.Core.Tests.Variables;
using Syringe.Web.Models.ValidationAttributes;

namespace Syringe.Web.Models
{
	public class CapturedVariableItem
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[ValidRegex(ErrorMessage = "Regex is not valid")]
		public string Regex { get; set; }

		[Required]
		[Display(Name = "Post Processor")]
		public VariablePostProcessorType PostProcessorType { get; set; }
	}
}
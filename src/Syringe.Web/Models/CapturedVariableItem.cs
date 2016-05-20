using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Syringe.Core.Tests.Variables;

namespace Syringe.Web.Models
{
    public class CapturedVariableItem
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Regex { get; set; }

        [Required]
        [Display(Name = "Post Processor")]
        public VariablePostProcessorType PostProcessorType { get; set; }
    }
}
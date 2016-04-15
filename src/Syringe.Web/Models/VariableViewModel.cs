using System.ComponentModel.DataAnnotations;

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
    }
}
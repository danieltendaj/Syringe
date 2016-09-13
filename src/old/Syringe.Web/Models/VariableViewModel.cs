using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
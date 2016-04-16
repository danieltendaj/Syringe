using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class CapturedVariableItem
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Regex { get; set; }
    }
}
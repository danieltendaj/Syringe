using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class HeaderItem
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
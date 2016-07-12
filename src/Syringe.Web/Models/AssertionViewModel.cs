using System.ComponentModel.DataAnnotations;
using Syringe.Core.Tests;
using Syringe.Web.ValidationAttributes;

namespace Syringe.Web.Models
{
    public class AssertionViewModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        [AssertionTypeValidation]
        public string Value { get; set; }

        [Display(Name = "Assertion Type")]
        public AssertionType AssertionType { get; set; }

        [Display(Name = "Assertion Method")]
        public AssertionMethod AssertionMethod { get; set; }

    }
}
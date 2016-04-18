using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class TestFileViewModel
    {
        [Required]
        public string Filename { get; set; }
        public string Xml { get; set; }
        public IEnumerable<TestViewModel> Tests { get; set; }
        public List<VariableViewModel> Variables { get; set; }
        public string[] Environments { get; set; }

		public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        public double PageNumbers { get; set; }
    }
}
using System.Collections.Generic;

namespace Syringe.Web.Models
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int NoOfResults { get; set; }
        public double PageNumbers { get; set; }
        public IEnumerable<string> Files { get; set; }
        public string[] Environments { get; set; }
    }
}
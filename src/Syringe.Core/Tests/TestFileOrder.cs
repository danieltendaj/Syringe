using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Tests
{
    public class TestFileOrder
    {
        public string Filename { get; set; }
        public List<TestPosition> Tests { get; set; }

        public TestFileOrder()
        {
            Tests = new List<TestPosition>();
        }
    }
}

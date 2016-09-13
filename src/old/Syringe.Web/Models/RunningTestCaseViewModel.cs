using System;
using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Web.Models
{
    public class RunningTestViewModel
    {
        public RunningTestViewModel(int index, string description, List<Assertion> assertions )
        {
            Position = index;
            Description = description;
            Assertions = assertions;
        }

        public int  Position { get; private set; }
        public string Description { get; private set; }
        public List<Assertion> Assertions {get; set; } 
    }
}
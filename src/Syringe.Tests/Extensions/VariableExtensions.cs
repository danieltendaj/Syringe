﻿using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Tests;
﻿using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Extensions
{
    public static class VariableExtensions
    {
        public static Variable ByName(this List<Variable> list, string name)
        {
            return list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string ValueByName(this List<Variable> list, string name)
        {
            Variable item = list.ByName(name);
            if (item != null)
            {
                return item.Value;
            }
            else
            {
                return "";
            }
        }
    }
}
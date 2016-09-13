using System;
using System.Collections.Generic;
using System.Linq;

namespace Syringe.Core.Extensions
{
    public static class Pager
    {
        public static IEnumerable<T> GetPaged<T>(this IEnumerable<T> collection, int noOfResults, int pageNumber)
        {
            return collection.Skip((pageNumber - 1) * noOfResults).Take(noOfResults);
        }

        public static double GetPageNumbersToShow<T>(this IEnumerable<T> collection, int noOfResults)
        {
            return Math.Ceiling((double)collection.Count() / noOfResults);
        }
    }
}

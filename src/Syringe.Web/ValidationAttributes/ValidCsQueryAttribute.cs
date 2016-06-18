using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CsQuery;

namespace Syringe.Web.ValidationAttributes
{
    public class ValidCsQueryAttribute :ValidationAttribute
    {
       
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            CQ cq = CQ.Create("");
            try
            {
                cq.Find(value.ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }

      
    }
}
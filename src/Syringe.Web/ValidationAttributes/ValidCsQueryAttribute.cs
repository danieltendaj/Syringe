using System.ComponentModel.DataAnnotations;
using CsQuery;

namespace Syringe.Web.ValidationAttributes
{
    public class ValidCsQueryAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                CQ cq = CQ.Create("");
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
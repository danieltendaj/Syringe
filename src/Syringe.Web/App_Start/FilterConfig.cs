﻿using System.Web.Mvc;

namespace Syringe.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
			filters.Add(new LogExceptionsAttribute());
		}
    }
}

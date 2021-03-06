﻿using SoulsClient.Filters;
using System.Web;
using System.Web.Mvc;

namespace SoulsClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new BasicAuthorizeAttribute());
        }
    }
}

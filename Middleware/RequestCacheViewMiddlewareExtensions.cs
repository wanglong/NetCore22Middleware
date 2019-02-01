﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace WebAppIISNginx.Middleware
{
    public static class RequestCacheViewMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCacheView(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCacheViewMiddleware>();
        }
    }
}
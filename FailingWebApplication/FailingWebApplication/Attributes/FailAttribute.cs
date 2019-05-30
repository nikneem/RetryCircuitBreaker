using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace FailingWebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FailAttribute : ActionFilterAttribute
    {



        public string Name { get; set; }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cache = (IMemoryCache)context.HttpContext.RequestServices.GetService(typeof(IMemoryCache));
            var cacheCount = 1;
            var key = $"{Name}";
            var allowExecute = true;

            if (cache.TryGetValue(key, out int count))
            {
                cacheCount = count + 1;
                allowExecute = count < 3;
            }

            cache.Set(key, cacheCount, TimeSpan.FromMinutes(1));

            if (!allowExecute)
            {
                context.Result = new JsonResult("Service failed");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}

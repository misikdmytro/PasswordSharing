using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Web.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HttpTraficMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpTraficMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var messageHandler = (IDbRepository<HttpMessage>)httpContext.RequestServices
                .GetService(typeof(IDbRepository<HttpMessage>));

            var url = httpContext.Request.GetDisplayUrl();

            await messageHandler.AddAsync(new HttpMessage
            {
                RequstedAt = DateTime.Now,
                Url = url,
                Method = httpContext.Request.Method
            });

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HttpTraficMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpTraficMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpTraficMiddleware>();
        }
    }
}

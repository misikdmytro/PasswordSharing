using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;

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
            var factory = (IContextFactory<ApplicationContext>)httpContext.RequestServices
                .GetService(typeof(IContextFactory<ApplicationContext>));

            var url = httpContext.Request.GetDisplayUrl();

            using (var context = factory.CreateContext())
            {
                var repository = new DbRepository<HttpMessage>(context);

                await repository.AddAsync(new HttpMessage
                {
                    RequstedAt = DateTime.Now,
                    Url = url,
                    Method = httpContext.Request.Method
                });
            }
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

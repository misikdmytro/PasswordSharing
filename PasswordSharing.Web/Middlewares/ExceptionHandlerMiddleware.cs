using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PasswordSharing.Web.Exceptions;

namespace PasswordSharing.Web.Middlewares
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlerMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (HttpResponseException ex)
			{
				httpContext.Response.Clear();
				httpContext.Response.StatusCode = (int)ex.Code;
				httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;

				await httpContext.Response.WriteAsync(ex.Message);
			}
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ExceptionHandlerMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlerMiddleware>();
		}
	}
}

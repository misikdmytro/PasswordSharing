using System;
using System.Net;

namespace PasswordSharing.Web.Exceptions
{
	public class HttpResponseException : Exception
	{
		public HttpStatusCode Code { get; }

		public HttpResponseException(HttpStatusCode code)
		{
			Code = code;
		}

		public HttpResponseException(HttpStatusCode code, string message) : base(message)
		{
			Code = code;
		}
	}
}

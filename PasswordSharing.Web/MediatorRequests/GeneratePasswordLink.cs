using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Exceptions;
using PasswordSharing.Models;
using PasswordSharing.Web.Exceptions;

namespace PasswordSharing.Web.MediatorRequests
{
	public class GeneratePasswordLinkRequest : IRequest<Password>
	{
		public string Password { get; }
		public int ExpiresIn { get; }

        public GeneratePasswordLinkRequest(string password, int expiresIn)
		{
			Password = password;
		    ExpiresIn = expiresIn;

		}
	}

	public class GeneratePasswordLinkHandler : IRequestHandler<GeneratePasswordLinkRequest, Password>
	{
		private readonly IPasswordEncryptor _passwordEncryptor;
	    private readonly IEventHandler<PasswordCreated> _eventHandler;
		private readonly ILogger<GeneratePasswordLinkHandler> _logger;

        public GeneratePasswordLinkHandler(IPasswordEncryptor passwordEncryptor, 
            IEventHandler<PasswordCreated> eventHandler, ILogger<GeneratePasswordLinkHandler> logger)
		{
			_passwordEncryptor = passwordEncryptor;
		    _eventHandler = eventHandler;
			_logger = logger;
		}

		public async Task<Password> Handle(GeneratePasswordLinkRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var password = _passwordEncryptor.Encode(request.Password,
					TimeSpan.FromSeconds(request.ExpiresIn));

				var model = new PasswordCreated(password);
				await _eventHandler.When(model);

				_logger.LogInformation($"Password with ID {password.Id} encrypted. Valid until {password.ExpiresAt}");
				_logger.LogDebug($"Password model - {JsonConvert.SerializeObject(password)}");

				return password;
			}
			catch (BadLengthException)
			{
				const string message = "Message is too long";
				_logger.LogError(message);

				throw new HttpResponseException(HttpStatusCode.BadRequest, message);
			}
		}
	}
}

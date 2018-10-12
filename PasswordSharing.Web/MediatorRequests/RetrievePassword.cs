using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Web.Exceptions;

namespace PasswordSharing.Web.MediatorRequests
{
	public class RetrievePasswordRequest : IRequest<string>
	{
		public int PasswordId { get; }
		public string Key { get; }

		public RetrievePasswordRequest(string key, int passwordId)
		{
			Key = key;
			PasswordId = passwordId;
		}
	}

	public class RetrievePasswordHandler : IRequestHandler<RetrievePasswordRequest, string>
	{
		private readonly IPasswordEncryptor _passwordEncryptor;
	    private readonly IDbRepository<Password> _passwordRepository;
	    private readonly IEventHandler<PasswordStatusChanged> _eventHandler;
		private readonly ILogger<RetrievePasswordHandler> _logger;

        public RetrievePasswordHandler(IPasswordEncryptor passwordEncryptor, 
            IDbRepository<Password> passwordRepository,
            IEventHandler<PasswordStatusChanged> eventHandler, 
	        ILogger<RetrievePasswordHandler> logger)
		{
			_passwordEncryptor = passwordEncryptor;
		    _passwordRepository = passwordRepository;
		    _eventHandler = eventHandler;
			_logger = logger;
		}

		public async Task<string> Handle(RetrievePasswordRequest request, CancellationToken cancellationToken)
		{
			var password = (await _passwordRepository.FindAsync(x => x.Id == request.PasswordId))
                .SingleOrDefault();

			if (password == null)
			{
				const string message = "Link not exists";
				_logger.LogError(message);

				throw new HttpResponseException(HttpStatusCode.BadRequest, message);
			}

			_logger.LogDebug($"Password model - {JsonConvert.SerializeObject(password)}");

			if (password.Status != PasswordStatus.Valid)
			{
				var message = PrepareErrorMessage(password);
				_logger.LogError(message);

				throw new HttpResponseException(HttpStatusCode.BadRequest, message);
			}

			if (password.ExpiresAt < DateTime.Now)
			{
                var model = new PasswordStatusChanged(password, PasswordStatus.Expired);
			    await _eventHandler.When(model);

				var message = PrepareErrorMessage(password);
				_logger.LogError(message);

				throw new HttpResponseException(HttpStatusCode.BadRequest, message);
			}
            
			password.Key = request.Key;

			try
			{
				_logger.LogInformation($"Trying to retrieve password with ID {password.Id}");

                var result = _passwordEncryptor.Decode(password);

                var model = new PasswordStatusChanged(password, PasswordStatus.Used);
			    await _eventHandler.When(model);

                return result;
			}
			catch (Exception)
			{
				const string message = "Incorrect link";
				_logger.LogError(message);

				throw new HttpResponseException(HttpStatusCode.BadRequest, message);
			}
		}

		private string PrepareErrorMessage(Password password)
		{
			switch (password.Status)
			{
				case PasswordStatus.Valid:
					throw new ArgumentException("Password is valid");
				case PasswordStatus.Expired:
					return $"Link expired at {password.ExpiresAt}";
				case PasswordStatus.Used:
					return "Link has been already used";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}

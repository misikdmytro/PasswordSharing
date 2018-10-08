using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
		private readonly IPasswordBuilder _passwordBuilder;
	    private readonly IDbRepository<Password> _passwordRepository;
	    private readonly IEventHandler<PasswordStatusChanged> _eventHandler;

        public RetrievePasswordHandler(IPasswordBuilder passwordBuilder, 
            IDbRepository<Password> passwordRepository,
            IEventHandler<PasswordStatusChanged> eventHandler)
		{
			_passwordBuilder = passwordBuilder;
		    _passwordRepository = passwordRepository;
		    _eventHandler = eventHandler;
		}

		public async Task<string> Handle(RetrievePasswordRequest request, CancellationToken cancellationToken)
		{
			var password = (await _passwordRepository.FindAsync(x => x.Id == request.PasswordId))
                .SingleOrDefault();

			if (password == null)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest, "Link not exists");
			}

		    if (password.Status != PasswordStatus.Valid)
		    {
		        throw new HttpResponseException(HttpStatusCode.BadRequest, "Link is not valid");
		    }

            if (password.ExpiresAt < DateTime.Now)
			{
                var model = new PasswordStatusChanged(password, PasswordStatus.Expired);
			    await _eventHandler.When(model);

				throw new HttpResponseException(HttpStatusCode.BadRequest, "Link expired");
			}
            
			password.Key = request.Key;

			try
			{
                var result = _passwordBuilder.Decode(password);

                var model = new PasswordStatusChanged(password, PasswordStatus.Used);
			    await _eventHandler.When(model);

                return result;
			}
			catch (Exception)
			{
                var model = new PasswordStatusChanged(password, PasswordStatus.Breaked);
			    await _eventHandler.When(model);

                throw new HttpResponseException(HttpStatusCode.BadRequest, "Incorrect link");
			}
		}
	}
}

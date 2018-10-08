using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

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
		private readonly IPasswordBuilder _passwordBuilder;
	    private readonly IEventHandler<PasswordCreated> _eventHandler;

        public GeneratePasswordLinkHandler(IPasswordBuilder passwordBuilder, 
            IEventHandler<PasswordCreated> eventHandler)
		{
			_passwordBuilder = passwordBuilder;
		    _eventHandler = eventHandler;
		}

		public async Task<Password> Handle(GeneratePasswordLinkRequest request, CancellationToken cancellationToken)
		{
			var password = _passwordBuilder.Encode(request.Password, 
                TimeSpan.FromSeconds(request.ExpiresIn));

		    var model = new PasswordCreated(password);
		    await _eventHandler.When(model);

			return password;
		}
	}
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Web.MediatorRequests
{
	public class GeneratePasswordLinkRequest : IRequest<Link>
	{
		public string Password { get; }

		public GeneratePasswordLinkRequest(string password)
		{
			Password = password;
		}
	}

	public class GeneratePasswordLinkHandler : IRequestHandler<GeneratePasswordLinkRequest, Link>
	{
		private readonly ILinkBuilder _linkBuilder;
		private readonly IPasswordBuilder _passwordBuilder;
		private readonly ILinkRepository _linkRepository;
		private readonly ExpirationTime _expirationTime;

		public GeneratePasswordLinkHandler(ILinkBuilder linkBuilder, IPasswordBuilder passwordBuilder, ExpirationTime expirationTime, ILinkRepository linkRepository)
		{
			_linkBuilder = linkBuilder;
			_passwordBuilder = passwordBuilder;
			_expirationTime = expirationTime;
			_linkRepository = linkRepository;
		}

		public async Task<Link> Handle(GeneratePasswordLinkRequest request, CancellationToken cancellationToken)
		{
			var password = _passwordBuilder.Encode(request.Password);
			var link = _linkBuilder.Build(password, _expirationTime);

			await _linkRepository.AddAsync(link);

			return link;
		}
	}
}

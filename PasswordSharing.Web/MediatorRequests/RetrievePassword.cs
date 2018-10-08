using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordSharing.Contracts;
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
		private readonly ILinkRepository _linkRepository;

		public RetrievePasswordHandler(IPasswordBuilder passwordBuilder, ILinkRepository linkRepository)
		{
			_passwordBuilder = passwordBuilder;
			_linkRepository = linkRepository;
		}

		public async Task<string> Handle(RetrievePasswordRequest request, CancellationToken cancellationToken)
		{
			var link = await _linkRepository.GetByPasswordId(request.PasswordId);

			if (link == null)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest, "Link not exists");
			}

			await _linkRepository.DeleteByPasswordId(request.PasswordId);

			if (link.ExpiresAt < DateTime.Now)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest, "Link expired");
			}

			var password = link.Password;
			password.Key = request.Key;

			try
			{
				return _passwordBuilder.Decode(password);
			}
			catch (Exception)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest, "Incorrect link");
			}
		}
	}
}

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PasswordSharing.Web.MediatorRequests;
using PasswordSharing.Web.Models;

namespace PasswordSharing.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PasswordController : ControllerBase
	{
		private readonly IMediator _mediator;

		public PasswordController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost("")]
		public async Task<IActionResult> Generate(PasswordModel password)
		{
			var request = new GeneratePasswordLinkRequest(password.Password);
			var result = await _mediator.Send(request);

			return Ok(Url.Link("GetPassword", new { passwordId = result.PasswordId, linkKey = result.LinkKey, controller = "password" }));
		}

		[HttpGet("{passwordId}/{linkKey}", Name = "GetPassword")]
		public async Task<IActionResult> Password(int passwordId, string linkKey)
		{
			var request = new RetrievePasswordRequest(linkKey, passwordId);
			var result = await _mediator.Send(request);

			return Ok(new PasswordModel { Password = result });
		}
	}
}

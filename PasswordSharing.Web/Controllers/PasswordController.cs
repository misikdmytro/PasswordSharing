﻿using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PasswordSharing.Web.MediatorRequests;
using PasswordSharing.Web.Models;

namespace PasswordSharing.Web.Controllers
{
	/// <summary>
	/// Password Controller
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class PasswordController : ControllerBase
	{
		private readonly IMediator _mediator;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="mediator">MediatoR</param>
		public PasswordController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Generate link for password sharing
		/// </summary>
		/// <param name="inModel">Password model</param>
		/// <response code="200">Returns share link to password</response>
		/// <response code="400">Message was too long</response>
		/// <response code="500">Internal Server Error</response>
		[HttpPost("", Name = "PasswordGenerator")]
		[ProducesResponseType(typeof(UrlModel), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> Generate([FromBody, BindRequired]PasswordInModel inModel)
		{
			var request = new GeneratePasswordLinkRequest(inModel.Password, inModel.ExpiresIn);
			var result = await _mediator.Send(request);

			var url = Url.Link("GetPassword", new
			{
				passwordId = result.Id,
				key = result.Key,
				controller = "password"
			});

			return Ok(new UrlModel { Url = url });
		}

		/// <summary>
		/// Decode password generated by 'PasswordGenerator' API
		/// </summary>
		/// <param name="passwordId">Password ID</param>
		/// <param name="key">Key provided by 'PasswordGenerator' API</param>
		/// <response code="200">Returns decoded password</response>
		/// <response code="400">Several reasons: link expired or incorrect URL used</response>
		/// <response code="500">Internal Server Error</response>
		[HttpGet("{passwordId}", Name = "GetPassword")]
		[ProducesResponseType(typeof(PasswordOutModel), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> Password([FromRoute, BindRequired]int passwordId,
			[FromQuery, BindRequired]string key)
		{
			var request = new RetrievePasswordRequest(key, passwordId);
			var result = await _mediator.Send(request);

			return Ok(new PasswordOutModel { Password = result });
		}
	}
}

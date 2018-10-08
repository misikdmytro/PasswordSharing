using System;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Services
{
	public class LinkBuilder : ILinkBuilder
	{
		private const int LinkIdLength = 16;
		private readonly IStringGenerator _stringGenerator;

		public LinkBuilder(IStringGenerator stringGenerator)
		{
			_stringGenerator = stringGenerator;
		}

		public Link Build(Password password, TimeSpan expiration)
		{
			var linkKey = _stringGenerator.Generate(LinkIdLength);

			return new Link
			{
				Password = password,
				ExpiresAt = DateTime.Now.Add(expiration),
				LinkKey = password.Key
			};
		}
	}
}

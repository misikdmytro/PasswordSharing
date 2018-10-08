using System;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface ILinkBuilder
	{
		Link Build(Password password, TimeSpan expiration);
	}
}
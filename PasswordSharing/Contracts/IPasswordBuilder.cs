using System;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface IPasswordBuilder
	{
		Password Encode(string password, TimeSpan expiration);
		string Decode(Password password);
	}
}
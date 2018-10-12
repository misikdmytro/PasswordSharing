using System;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface IPasswordEncryptor
	{
		Password Encode(string password, TimeSpan expiration);
		string Decode(Password password);
	}
}
using System.Security.Cryptography;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface IPasswordBuilder
	{
		Password Encode(string password);
		string Decode(Password password);
	}
}
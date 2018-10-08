using System;
using PasswordSharing.Contracts;

namespace PasswordSharing.Services
{
	public class RandomStringGenerator : IStringGenerator
	{
		private const string AvailableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		public string Generate(int length)
		{
			var stringChars = new char[length];
			var random = new Random();

			for (var i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = AvailableChars[random.Next(AvailableChars.Length)];
			}

			return new string(stringChars);
		}
	}
}

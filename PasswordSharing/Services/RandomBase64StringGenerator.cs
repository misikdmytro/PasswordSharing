using System;
using System.Text;
using PasswordSharing.Contracts;

namespace PasswordSharing.Services
{
	public class RandomBase64StringGenerator : IRandomBase64StringGenerator
	{
		private const string AvailableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		public string Generate(int originalLength)
		{
			var stringChars = new char[originalLength];
			var random = new Random();

			for (var i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = AvailableChars[random.Next(AvailableChars.Length)];
			}

			return Convert.ToBase64String(Encoding.UTF8.GetBytes(stringChars));
		}
	}
}

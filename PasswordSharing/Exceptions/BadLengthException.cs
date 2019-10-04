using System;

namespace PasswordSharing.Exceptions
{
	public class BadLengthException : Exception
	{
        public BadLengthException(string message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using PasswordSharing.Contracts;

namespace PasswordSharing.Algorithms
{
	public class RSAAlgoParameters : IRSAAlgoParameters
	{
		public RSAParameters PublicKey { get; }
		public RSAParameters PrivateKey { get; }

		public RSAAlgoParameters(RSAParameters publicKey, RSAParameters privateKey)
		{
			PublicKey = publicKey;
			PrivateKey = privateKey;
		}
	}
}

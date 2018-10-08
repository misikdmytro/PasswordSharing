using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Algorithms
{
	public class RsaParametersBuilder : IRsaParametersBuilder
	{
		public RSAParameters Build(PublicKey publicKey, PrivateKey privateKey)
		{
			return new RSAParameters
			{
				P = privateKey.P,
				InverseQ = privateKey.InverseQ,
				DQ = privateKey.DQ,
				DP = privateKey.DP,
				D = privateKey.D,
				Modulus = publicKey.Modulus,
				Exponent = publicKey.Exponent,
				Q = privateKey.Q
			};
		}

		public RSAParameters Build(PublicKey publicKey)
		{
			return new RSAParameters
			{
				Modulus = publicKey.Modulus,
				Exponent = publicKey.Exponent,
			};
		}

		public RSAParameters Build(string str)
		{
			using (var sr = new StringReader(str))
			{
				var xs = new XmlSerializer(typeof(RSAParameters));
				var @params = (RSAParameters)xs.Deserialize(sr);

				return @params;
			}
		}
	}
}

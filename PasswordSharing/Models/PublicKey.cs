using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace PasswordSharing.Models
{
	public class PublicKey
	{
		public byte[] Exponent { get; set; }
		public byte[] Modulus { get; set; }

		public RSAParameters Convert()
		{
			return new RSAParameters
			{
				Exponent = Exponent,
				Modulus = Modulus
			};
		}

		public override string ToString()
		{
			using (var sw = new StringWriter())
			{
				var xs = new XmlSerializer(typeof(PublicKey));
				xs.Serialize(sw, this);
				return sw.ToString();
			}
		}

		public static PublicKey FromString(string str)
		{
			using (var sr = new StringReader(str))
			{
				var xs = new XmlSerializer(typeof(PublicKey));
				var pubKey = (PublicKey) xs.Deserialize(sr);

				return pubKey;
			}
		}
	}
}

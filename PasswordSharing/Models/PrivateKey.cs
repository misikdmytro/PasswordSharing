using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace PasswordSharing.Models
{
	public class PrivateKey
	{
		public byte[] D { get; set; }
		public byte[] DP { get; set; }
		public byte[] DQ { get; set; }
		public byte[] InverseQ { get; set; }
		public byte[] P { get; set; }
		public byte[] Q { get; set; }

		public RSAParameters Convert()
		{
			return new RSAParameters
			{
				D = D,
				DP = DP,
				DQ = DQ,
				InverseQ = InverseQ,
				P = P,
				Q = Q
			};
		}

		public static PrivateKey FromString(string str)
		{
			using (var sr = new StringReader(str))
			{
				var xs = new XmlSerializer(typeof(PrivateKey));
				var key = (PrivateKey)xs.Deserialize(sr);

				return key;
			}
		}
	}
}

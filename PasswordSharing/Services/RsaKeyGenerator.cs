using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using PasswordSharing.Constants;
using PasswordSharing.Contracts;

namespace PasswordSharing.Services
{
    public class RsaKeyGenerator : IRsaKeyGenerator
    {
        public RSAParameters GenerateKey()
        {
            using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
            {
                try
                {
                    return csp.ExportParameters(true);
                }
                finally
                {
                    csp.PersistKeyInCsp = false;
                    csp.Clear();
                }
            }
        }

        public string ToString(RSAParameters parameters)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, parameters);

                var keyStr = sw.ToString();

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(keyStr));
            }
        }

        public RSAParameters FromString(string key)
        {
            var keyStr = Encoding.UTF8.GetString(Convert.FromBase64String(key));

            using (var sr = new StringReader(keyStr))
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                return (RSAParameters) xs.Deserialize(sr);
            }
        }
    }
}

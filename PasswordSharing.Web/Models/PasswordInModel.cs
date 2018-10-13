using Newtonsoft.Json;

namespace PasswordSharing.Web.Models
{
    /// <summary>
    /// Input password model
    /// </summary>
	public class PasswordInModel
	{
        /// <summary>
        /// Passwords to encode
        /// </summary>
        [JsonProperty(Required = Required.Always)]
		public string[] Passwords { get; set; }
        /// <summary>
        /// Expiration time (in seconds)
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int ExpiresIn { get; set; }
	}
}

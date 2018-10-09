using Newtonsoft.Json;

namespace PasswordSharing.Web.Models
{
    /// <summary>
    /// Input password model
    /// </summary>
	public class PasswordInModel
	{
        /// <summary>
        /// Password to encode
        /// </summary>
        [JsonProperty(Required = Required.Always)]
		public string Password { get; set; }
        /// <summary>
        /// Expiration time (in seconds)
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int ExpiresIn { get; set; }
	}
}

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
		public string Password { get; set; }
        /// <summary>
        /// Expiration time (in seconds)
        /// </summary>
        public int ExpiresIn { get; set; }
	}
}

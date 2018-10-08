namespace PasswordSharing.Models
{
	public class Password
	{
		public int Id { get; set; }
		public string PublicKey { get; set; }
		public string Encoded { get; set; }
	}
}

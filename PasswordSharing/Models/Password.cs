namespace PasswordSharing.Models
{
	public class Password : IIDentifiable
	{
		public int Id { get; set; }
		public string Encoded { get; set; }
		public string Key { get; set; }
	}
}

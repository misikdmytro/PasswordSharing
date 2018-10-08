using System;

namespace PasswordSharing.Models
{
	public class Password : IIDentifiable
	{
		public int Id { get; set; }
		public string Encoded { get; set; }
		public string Key { get; set; }
	    public DateTime ExpiresAt { get; set; }
	    public PasswordStatus Status { get; set; }
    }
}

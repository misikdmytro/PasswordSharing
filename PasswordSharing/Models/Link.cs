using System;

namespace PasswordSharing.Models
{
	public class Link : IIDentifiable
	{
		public int Id { get; set; }
		public string LinkKey { get; set; }
		public Password Password { get; set; }
		public int PasswordId { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}

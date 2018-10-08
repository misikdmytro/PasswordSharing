using System;

namespace PasswordSharing.Models
{
	public class Link
	{
		public int Id { get; set; }
		public Password Password { get; set; }
		public int PasswordId { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}

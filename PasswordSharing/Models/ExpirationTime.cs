using System;

namespace PasswordSharing.Models
{
	public class ExpirationTime
	{
		public int Days { get; set; }
		public int Hours { get; set; }
		public int Minutes { get; set; }
		public int Seconds { get; set; }
		public int Milliseconds { get; set; }

		public static implicit operator TimeSpan(ExpirationTime time)
		{
			return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
		}
	}
}

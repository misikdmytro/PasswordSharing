using System;
using System.Collections.Generic;

namespace PasswordSharing.Models
{
    public class PasswordGroup : IIDentifiable
    {
        public int Id { get; set; }
        public ICollection<Password> Passwords { get; set; }
	    public DateTime ExpiresAt { get; set; }
	    public PasswordStatus Status { get; set; }
    }
}

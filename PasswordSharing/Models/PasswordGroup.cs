using System;
using System.Collections.Generic;

namespace PasswordSharing.Models
{
    public class PasswordGroup : IIDentifiable
    {
        public Guid Id { get; set; }
        public ICollection<Password> Passwords { get; set; }
    }
}

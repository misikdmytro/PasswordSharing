using System;

namespace PasswordSharing.Models
{
    public class HttpMessage : IIDentifiable
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public DateTime RequstedAt { get; set; }
    }
}

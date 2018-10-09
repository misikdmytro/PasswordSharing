namespace PasswordSharing.Models
{
    public class Event : IIDentifiable
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
		public Password Password { get; set; }
		public int PasswordId { get; set; }
    }
}

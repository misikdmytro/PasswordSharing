namespace PasswordSharing.Models
{
    public class Event : IIDentifiable
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}

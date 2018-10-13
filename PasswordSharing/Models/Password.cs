namespace PasswordSharing.Models
{
	public class Password : IIDentifiable
	{
		public int Id { get; set; }
		public string Encoded { get; set; }
        public byte[] RowVersion { get; set; }
        public int PasswordGroupId { get; set; }
    }
}

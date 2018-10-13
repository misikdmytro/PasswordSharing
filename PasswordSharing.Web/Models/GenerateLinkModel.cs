namespace PasswordSharing.Web.Models
{
    /// <summary>
    /// 'Generate password' response model
    /// </summary>
    public class GenerateLinkModel
    {
        /// <summary>
        /// Password group ID
        /// </summary>
        public int PasswordGroupId { get; set; }
        /// <summary>
        /// Key used in link
        /// </summary>
        public string Key { get; set; }
    }
}

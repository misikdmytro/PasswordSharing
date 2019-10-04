using System;

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
        public Guid PasswordGroupId { get; set; }
        /// <summary>
        /// Key used in link
        /// </summary>
        public string Key { get; set; }
    }
}

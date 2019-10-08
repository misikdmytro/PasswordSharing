using System;

namespace PasswordSharing.Models
{
    public class PasswordGroupKey : ICacheKey
    {
        public Guid GroupId { get; }

        public PasswordGroupKey(Guid groupId)
        {
            GroupId = groupId;
        }

        public string ExtractKey()
        {
            return $"password_group_{GroupId}";
        }
    }
}

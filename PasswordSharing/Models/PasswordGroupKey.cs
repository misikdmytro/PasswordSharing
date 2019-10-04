using System;

namespace PasswordSharing.Models
{
    public class PasswordGroupKey : ICacheKey
    {
        private readonly Guid _groupId;

        public PasswordGroupKey(Guid groupId)
        {
            _groupId = groupId;
        }

        public string ExtractKey()
        {
            return $"password_group_{_groupId}";
        }
    }
}

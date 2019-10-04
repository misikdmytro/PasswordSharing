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

        public override string ToString()
        {
            return $"password_group_{_groupId}";
        }
    }
}

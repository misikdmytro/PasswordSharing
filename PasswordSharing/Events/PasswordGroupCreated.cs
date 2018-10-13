using System;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class PasswordGroupCreated : IGroupEvent
    {
        public int PasswordGroupId => PasswordGroup.Id;
        public DateTime CreatedAt { get; }

        internal PasswordGroup PasswordGroup { get; }

        public PasswordGroupCreated(PasswordGroup passwordGroup)
        {
            PasswordGroup = passwordGroup;
            CreatedAt = DateTime.Now;
        }
    }
}

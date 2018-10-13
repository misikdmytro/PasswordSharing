using System;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class PasswordGroupStatusChanged : IGroupEvent
    {
        public int PasswordGroupId => PasswordGroup.Id;
        public string Previous { get; }
        public string Current => NewStatus.ToString();
        public DateTime ChangedAt { get; }

        internal PasswordGroup PasswordGroup { get; }
        internal PasswordStatus NewStatus { get; }

        public PasswordGroupStatusChanged(PasswordGroup passwordGroup, PasswordStatus @new)
        {
            NewStatus = @new;
            PasswordGroup = passwordGroup;
            Previous = passwordGroup.Status.ToString();
            ChangedAt = DateTime.Now;
        }
    }
}

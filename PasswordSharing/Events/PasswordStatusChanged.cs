using System;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class PasswordStatusChanged : IEvent
    {
        public string Previous { get; }
        public string Current => NewStatus.ToString();
        public DateTime ChangedAt { get; }
        public int PasswordId => Password.Id;

        internal Password Password { get; }
        internal PasswordStatus NewStatus { get; }

        public PasswordStatusChanged(Password password, PasswordStatus @new)
        {
            NewStatus = @new;
            Password = password;
            Previous = password.Status.ToString();
            ChangedAt = DateTime.Now;
        }
    }
}

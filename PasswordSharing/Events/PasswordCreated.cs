using System;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class PasswordCreated : IEvent
    {
        public int PasswordId => Password.Id;
        internal Password Password { get; }
        public DateTime CreatedAt { get; }

        public PasswordCreated(Password password)
        {
            Password = password;
            CreatedAt = DateTime.Now;
        }
    }
}

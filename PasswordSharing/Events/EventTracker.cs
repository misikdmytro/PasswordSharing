using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PasswordSharing.Contracts;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class EventTracker : IEventTracker
    {
        private readonly IDbRepository<Event> _eventRepository;

        public EventTracker(IDbRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event> Register(object description)
        {
            var @event = new Event
            {
                Type = description.GetType().Name,
                Description = JsonConvert.SerializeObject(description)
            };

            await _eventRepository.AddAsync(@event);

            return @event;
        }
    }
}

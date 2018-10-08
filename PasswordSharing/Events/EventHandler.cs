using System.Threading.Tasks;
using PasswordSharing.Contracts;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Events
{
    public class EventHandler : IEventHandler<PasswordCreated>,
        IEventHandler<PasswordStatusChanged>
    {
        private readonly IEventTracker _eventTracker;
        private readonly IDbRepository<Password> _repository;

        public EventHandler(IEventTracker eventTracker, IDbRepository<Password> repository)
        {
            _eventTracker = eventTracker;
            _repository = repository;
        }

        public async Task When(PasswordCreated @event)
        {
            await _repository.AddAsync(@event.Password);
            await _eventTracker.Register(@event);
        }

        public async Task When(PasswordStatusChanged @event)
        {
            @event.Password.Status = @event.NewStatus;

            await _repository.UpdateAsync(@event.Password);
            await _eventTracker.Register(@event);
        }
    }
}

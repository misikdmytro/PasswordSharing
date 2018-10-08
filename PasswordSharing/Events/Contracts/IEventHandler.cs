using System.Threading.Tasks;

namespace PasswordSharing.Events.Contracts
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task When(TEvent @event);
    }
}

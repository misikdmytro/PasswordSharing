using System.Threading.Tasks;

namespace PasswordSharing.Events.Contracts
{
    public interface IGroupEventHandler<in TEvent>
        where TEvent : IGroupEvent
    {
        Task When(TEvent @event);
    }
}

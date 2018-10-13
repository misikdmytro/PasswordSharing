using System.Threading.Tasks;
using PasswordSharing.Models;

namespace PasswordSharing.Events.Contracts
{
    public interface IEventTracker
    {
        Task<Event> Register(IGroupEvent description);
    }
}

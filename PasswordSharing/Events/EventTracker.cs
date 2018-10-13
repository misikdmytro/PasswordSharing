using System.Threading.Tasks;
using Newtonsoft.Json;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;

namespace PasswordSharing.Events
{
	public class EventTracker : IEventTracker
	{
	    private readonly IContextFactory<ApplicationContext> _factory;

	    public EventTracker(IContextFactory<ApplicationContext> factory)
	    {
	        _factory = factory;
	    }

		public async Task<Event> Register(IGroupEvent description)
		{
			var @event = new Event
			{
				Type = description.GetType().Name,
				Description = JsonConvert.SerializeObject(description),
			    PasswordGroupId = description.PasswordGroupId
			};

		    using (var context = _factory.CreateContext())
		    {
                var eventRepository = new DbRepository<Event>(context);
		        await eventRepository.AddAsync(@event);

		        return @event;
            }
		}
	}
}

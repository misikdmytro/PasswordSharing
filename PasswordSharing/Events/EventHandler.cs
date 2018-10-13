using System.Threading.Tasks;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;

namespace PasswordSharing.Events
{
    public class EventHandler : IGroupEventHandler<PasswordGroupCreated>,
        IGroupEventHandler<PasswordGroupStatusChanged>
    {
        private readonly IEventTracker _eventTracker;
        private readonly IRandomBase64StringGenerator _stringGenerator;
        private readonly IContextFactory<ApplicationContext> _factory;

        public EventHandler(IEventTracker eventTracker, IContextFactory<ApplicationContext> factory,
            IRandomBase64StringGenerator stringGenerator)
        {
            _eventTracker = eventTracker;
            _factory = factory;
            _stringGenerator = stringGenerator;
        }

        public async Task When(PasswordGroupCreated @event)
        {
            using (var context = _factory.CreateContext())
            {
                var repository = new DbRepository<PasswordGroup>(context);
                await repository.AddAsync(@event.PasswordGroup);
            }

            await _eventTracker.Register(@event);
        }

        public async Task When(PasswordGroupStatusChanged @event)
        {
            using (var context = _factory.CreateContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var passwordRepository = new DbRepository<Password>(context);

                foreach (var password in @event.PasswordGroup.Passwords)
                {
                    password.Encoded = _stringGenerator.Generate(password.Encoded.Length);
                    await passwordRepository.UpdateAsync(password);
                }

                @event.PasswordGroup.Status = @event.NewStatus;

                var repository = new DbRepository<PasswordGroup>(context);
                await repository.UpdateAsync(@event.PasswordGroup);

                transaction.Commit();
            }

            await _eventTracker.Register(@event);
        }
    }
}

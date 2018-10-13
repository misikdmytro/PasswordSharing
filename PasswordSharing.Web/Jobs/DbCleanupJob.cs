using System;
using System.Data;
using FluentScheduler;
using Microsoft.Extensions.Logging;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;

namespace PasswordSharing.Web.Jobs
{
    public class DbCleanupJob : IJob
    {
        private readonly IGroupEventHandler<PasswordGroupStatusChanged> _eventHandler;
        private readonly ILogger<DbCleanupJob> _logger;
        private readonly IContextFactory<ApplicationContext> _contextFactory;

        public DbCleanupJob(IGroupEventHandler<PasswordGroupStatusChanged> eventHandler, ILogger<DbCleanupJob> logger,
            IContextFactory<ApplicationContext> contextFactory)
        {
            _eventHandler = eventHandler;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async void Execute()
        {
            using (var context = _contextFactory.CreateContext())
            {
                var passwordRepository = new PasswordGroupRepository(context);
                var expired = await passwordRepository.FindAsync(x => x.ExpiresAt < DateTime.Now &&
                                                                      x.Status == PasswordStatus.Valid);

                foreach (var passwordGroup in expired)
                {
                    try
                    {
                        await _eventHandler.When(new PasswordGroupStatusChanged(passwordGroup,
                            PasswordStatus.Expired));
                    }
                    catch (DBConcurrencyException ex)
                    {
                        // just ignore it
                        _logger.LogError(ex.Message);
                    }
                }

                _logger.LogInformation($"DB Updated - {expired.Length} password group(s) expired");
            }
        }
    }
}

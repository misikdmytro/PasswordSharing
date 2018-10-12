using System;
using System.Data;
using FluentScheduler;
using Microsoft.Extensions.Logging;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Web.Jobs
{
    public class DbCleanupJob : IJob
    {
        private readonly IEventHandler<PasswordStatusChanged> _eventHandler;
        private readonly IDbRepository<Password> _passwordRepository;
        private readonly ILogger<DbCleanupJob> _logger;

        public DbCleanupJob(IEventHandler<PasswordStatusChanged> eventHandler, ILogger<DbCleanupJob> logger,
            IDbRepository<Password> passwordRepository)
        {
            _eventHandler = eventHandler;
            _logger = logger;
            _passwordRepository = passwordRepository;
        }

        public async void Execute()
        {
            var expired = await _passwordRepository.FindAsync(x => x.ExpiresAt < DateTime.Now && x.Status == PasswordStatus.Valid);

            foreach (var password in expired)
            {
                try
                {
                    await _eventHandler.When(new PasswordStatusChanged(password, PasswordStatus.Expired));
                }
                catch (DBConcurrencyException ex)
                {
                    // just ignore it
                    _logger.LogError(ex.Message);
                }
            }

            _logger.LogInformation($"DB Updated - {expired.Length} password(s) expired");
        }
    }
}

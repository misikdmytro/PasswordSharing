using System;
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
		private readonly IDbRepository<Password> _repository;
		private readonly ILogger<DbCleanupJob> _logger;

		public DbCleanupJob(IEventHandler<PasswordStatusChanged> eventHandler, IDbRepository<Password> repository, ILogger<DbCleanupJob> logger)
		{
			_eventHandler = eventHandler;
			_repository = repository;
			_logger = logger;
		}

		public async void Execute()
		{
			var expired = await _repository.FindAsync(x => x.ExpiresAt < DateTime.Now && x.Status == PasswordStatus.Valid);
			foreach (var password in expired)
			{
				await _eventHandler.When(new PasswordStatusChanged(password, PasswordStatus.Expired));
			}

			_logger.LogInformation($"DB Updated - {expired.Length} password(s) expired");
		}
	}
}

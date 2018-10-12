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
		private readonly IConcurrentHelper<Password> _concurrentHelper;
		private readonly ILogger<DbCleanupJob> _logger;

		public DbCleanupJob(IEventHandler<PasswordStatusChanged> eventHandler, ILogger<DbCleanupJob> logger,
			IConcurrentHelper<Password> concurrentHelper)
		{
			_eventHandler = eventHandler;
			_logger = logger;
			_concurrentHelper = concurrentHelper;
		}

		public async void Execute()
		{
			var expired = await _concurrentHelper.FindAsync(x => x.ExpiresAt < DateTime.Now && x.Status == PasswordStatus.Valid);
			try
			{
				foreach (var password in expired)
				{
					await _eventHandler.When(new PasswordStatusChanged(password.Entity, PasswordStatus.Expired));
				}

				_logger.LogInformation($"DB Updated - {expired.Length} password(s) expired");
			}
			finally
			{
				foreach (var lockWrapper in expired)
				{
					lockWrapper.Dispose();
				}
			}
		}
	}
}

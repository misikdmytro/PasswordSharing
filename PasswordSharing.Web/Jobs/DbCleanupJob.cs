using System;
using FluentScheduler;
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

		public DbCleanupJob(IEventHandler<PasswordStatusChanged> eventHandler, IDbRepository<Password> repository)
		{
			_eventHandler = eventHandler;
			_repository = repository;
		}

		public async void Execute()
		{
			var expired = await _repository.FindAsync(x => x.ExpiresAt < DateTime.Now && x.Status == PasswordStatus.Valid);
			foreach (var password in expired)
			{
				await _eventHandler.When(new PasswordStatusChanged(password, PasswordStatus.Expired));
			}
			Console.WriteLine("DB Updated");
		}
	}
}

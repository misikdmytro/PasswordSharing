using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PasswordSharing.Contracts;
using PasswordSharing.Locks;
using PasswordSharing.Models;

namespace PasswordSharing.Repositories
{
	public class ConcurrentHelper<TEntity> : IConcurrentHelper<TEntity>
		where TEntity : class, IIDentifiable
	{
		protected static readonly IDictionary<int, SemaphoreSlim> Semaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
		protected static readonly object Lock = new object();

		protected readonly IDbRepository<TEntity> DbRepository;

		public ConcurrentHelper(IDbRepository<TEntity> dbRepository)
		{
			DbRepository = dbRepository;
		}

		public async Task<LockWrapper<TEntity>[]> FindAsync(Expression<Func<TEntity, bool>> predicate)
		{
			var entities = await DbRepository.FindAsync(predicate);

			foreach (var entity in entities)
			{
				var semaphore = GetSemaphore(entity.Id);
				await semaphore.WaitAsync();
			}

			var newEntities = await DbRepository.FindAsync(x => entities.Any(e => e.Id == x.Id));

			var locks = new List<LockWrapper<TEntity>>();

			foreach (var entity in newEntities)
			{
				var semaphore = GetSemaphore(entity.Id);
				locks.Add(LockWrapper<TEntity>.Create(entity, () => semaphore.Release()));
			}

			return locks.ToArray();
		}

		public async Task<LockWrapper<TEntity>> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
		{
			var entity = await DbRepository.SingleOrDefaultAsync(predicate);

			if (entity == null)
			{
				return LockWrapper<TEntity>.Empty;
			}

			var semaphore = GetSemaphore(entity.Id);
			await semaphore.WaitAsync();

			entity = await DbRepository.SingleOrDefaultAsync(predicate);

			return LockWrapper<TEntity>.Create(entity, () => semaphore.Release());
		}

		private static SemaphoreSlim GetSemaphore(int id)
		{
			SemaphoreSlim semaphore;
			lock (Lock)
			{
				Semaphores.TryGetValue(id, out semaphore);
				if (semaphore == null)
				{
					semaphore = new SemaphoreSlim(1, 1);
					Semaphores.Add(id, semaphore);
				}
			}

			return semaphore;
		}
	}
}

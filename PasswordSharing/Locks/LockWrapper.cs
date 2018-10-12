using System;

namespace PasswordSharing.Locks
{
	public class LockWrapper<TEntity> : IDisposable
	{
		public static readonly LockWrapper<TEntity> Empty = new LockWrapper<TEntity>(default(TEntity), () => { });

		private readonly Action _unlockAction;
		public TEntity Entity { get; protected set; }

		protected LockWrapper(TEntity entity, Action unlockAction)
		{
			Entity = entity;
			_unlockAction = unlockAction;
		}

		public void Dispose()
		{
			_unlockAction();
			Entity = default(TEntity);
		}

		public static LockWrapper<TEntity> Create(TEntity resource, Action unlockAction)
		{
			return new LockWrapper<TEntity>(resource, unlockAction);
		}
	}
}

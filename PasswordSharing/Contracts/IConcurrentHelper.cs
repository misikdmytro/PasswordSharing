using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PasswordSharing.Locks;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface IConcurrentHelper<TEntity>
		where TEntity : IIDentifiable
	{
		Task<LockWrapper<TEntity>[]> FindAsync(Expression<Func<TEntity, bool>> predicate);
		Task<LockWrapper<TEntity>> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
	}
}
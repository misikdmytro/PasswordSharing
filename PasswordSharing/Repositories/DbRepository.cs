using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Repositories
{
	public class DbRepository<TEntity> : IDbRepository<TEntity>
			where TEntity : class, IIDentifiable
	{
		private readonly DbContextOptions<ApplicationContext> _contextOptions;

		public DbRepository(DbContextOptions<ApplicationContext> contextOptions)
		{
			_contextOptions = contextOptions;
		}

		public async Task<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate)
		{
			using (var context = GetContext())
			{
				return await GetQuery(context).Where(predicate).ToArrayAsync();
			}
		}

		public async Task AddAsync(TEntity entity)
		{
			using (var context = GetContext())
			{
				await context.Set<TEntity>().AddAsync(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task UpdateAsync(TEntity entity)
		{
			using (var context = GetContext())
			{
				context.Entry(entity).State = EntityState.Modified;
				await context.SaveChangesAsync();
			}
		}

		public async Task RemoveAsync(int id)
		{
			using (var context = GetContext())
			{
				var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
				context.Set<TEntity>().Remove(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task ClearAsync()
		{
			using (var context = GetContext())
			{
				foreach (var entity in context.Set<TEntity>())
				{
					context.Remove(entity);
				}

				await context.SaveChangesAsync();
			}
		}

		protected ApplicationContext GetContext()
		{
			return new ApplicationContext(_contextOptions);
		}

		protected virtual IQueryable<TEntity> GetQuery(ApplicationContext context)
		{
			return context.Set<TEntity>();
		}
	}
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Repositories
{
	public class LinkRepository : DbRepository<Link>, ILinkRepository
	{
		public LinkRepository(DbContextOptions<ApplicationContext> contextOptions) : base(contextOptions)
		{
		}

		public async Task<Link> GetByPasswordId(int passwordId)
		{
			using (var context = GetContext())
			{
				return await GetQuery(context).SingleOrDefaultAsync(x => x.PasswordId == passwordId);
			}
		}

		public async Task DeleteByPasswordId(int passwordId)
		{
			using (var context = GetContext())
			{
				var entity = await GetQuery(context).FirstOrDefaultAsync(x => x.PasswordId == passwordId);
				context.Set<Link>().Remove(entity);
				await context.SaveChangesAsync();
			}
		}

		protected override IQueryable<Link> GetQuery(ApplicationContext context)
		{
			return base.GetQuery(context).Include(x => x.Password);
		}
	}
}

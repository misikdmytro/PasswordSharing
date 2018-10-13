using System.Linq;
using Microsoft.EntityFrameworkCore;
using PasswordSharing.Contexts;
using PasswordSharing.Models;

namespace PasswordSharing.Repositories
{
    public class PasswordGroupRepository : DbRepository<PasswordGroup>
    {
        public PasswordGroupRepository(ApplicationContext context) : base(context)
        {
        }

        protected override IQueryable<PasswordGroup> GetQuery(ApplicationContext context)
        {
            return base.GetQuery(context).Include(x => x.Passwords);
        }
    }
}

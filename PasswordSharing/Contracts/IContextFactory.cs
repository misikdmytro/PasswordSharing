using Microsoft.EntityFrameworkCore;

namespace PasswordSharing.Contracts
{
    public interface IContextFactory<out TContext>
        where TContext : DbContext
    {
        TContext CreateContext();
    }
}
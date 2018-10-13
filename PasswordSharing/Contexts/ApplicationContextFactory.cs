using Microsoft.EntityFrameworkCore;
using PasswordSharing.Contracts;

namespace PasswordSharing.Contexts
{
    public class ApplicationContextFactory : IContextFactory<ApplicationContext>
    {
        private readonly DbContextOptions _options;

        public ApplicationContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public ApplicationContext CreateContext()
        {
            return new ApplicationContext(_options);
        }
    }
}

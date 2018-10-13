using Microsoft.EntityFrameworkCore;
using PasswordSharing.Configurations;
using PasswordSharing.Models;

namespace PasswordSharing.Contexts
{
	public sealed class ApplicationContext : DbContext
	{
		public DbSet<Password> Passwords { get; set; }
		public DbSet<PasswordGroup> PasswordGroups { get; set; }
        public DbSet<Event> Events { get; set; }
		public DbSet<HttpMessage> HttpMessages { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new PasswordEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new PasswordGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EventEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new HttpMessageEntityTypeConfiguration());
		}
	}
}

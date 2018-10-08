using Microsoft.EntityFrameworkCore;
using PasswordSharing.Configurations;
using PasswordSharing.Models;

namespace PasswordSharing.Contexts
{
	public class ApplicationContext : DbContext
	{
		public DbSet<Password> Passwords { get; set; }
		public DbSet<Link> Links { get; set; }

		public ApplicationContext(DbContextOptions options) : base(options)
		{
			Database.Migrate();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new PasswordEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new LinkEntityTypeConfiguration());
		}
	}
}

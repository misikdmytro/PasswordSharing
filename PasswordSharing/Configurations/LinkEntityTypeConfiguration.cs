using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordSharing.Models;

namespace PasswordSharing.Configurations
{
	public class LinkEntityTypeConfiguration : IEntityTypeConfiguration<Link>
	{
		public void Configure(EntityTypeBuilder<Link> builder)
		{
			builder.HasKey(l => l.Id);
			builder.HasOne(l => l.Password)
				.WithOne()
				.HasForeignKey<Link>(l => l.PasswordId);
			builder.Property(l => l.ExpiresAt).IsRequired();
		}
	}
}

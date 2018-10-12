using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordSharing.Models;

namespace PasswordSharing.Configurations
{
	public class PasswordEntityTypeConfiguration : IEntityTypeConfiguration<Password>
	{
		public void Configure(EntityTypeBuilder<Password> builder)
		{
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Encoded).IsRequired();
		    builder.Property(p => p.Status).IsRequired();
		    builder.Property(p => p.ExpiresAt).IsRequired();
            builder.Ignore(p => p.Key);
		    builder.Property(x => x.RowVersion).IsRowVersion();
		}
	}
}

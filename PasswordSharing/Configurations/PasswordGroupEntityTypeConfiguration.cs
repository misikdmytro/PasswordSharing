using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordSharing.Models;

namespace PasswordSharing.Configurations
{
    public class PasswordGroupEntityTypeConfiguration : IEntityTypeConfiguration<PasswordGroup>
    {
        public void Configure(EntityTypeBuilder<PasswordGroup> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Passwords).WithOne().HasForeignKey(x => x.PasswordGroupId);
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.ExpiresAt).IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordSharing.Models;

namespace PasswordSharing.Configurations
{
    public class HttpMessageEntityTypeConfiguration : IEntityTypeConfiguration<HttpMessage>
    {
        public void Configure(EntityTypeBuilder<HttpMessage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RequstedAt).IsRequired();
            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.Method).IsRequired();
        }
    }
}

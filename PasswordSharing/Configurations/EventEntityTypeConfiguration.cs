using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordSharing.Models;

namespace PasswordSharing.Configurations
{
    public class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Type).IsRequired();
        }
    }
}

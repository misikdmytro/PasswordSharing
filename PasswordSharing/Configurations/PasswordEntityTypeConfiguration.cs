﻿using Microsoft.EntityFrameworkCore;
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
			builder.Property(p => p.PublicKey).IsRequired();
		}
	}
}
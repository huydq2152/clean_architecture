﻿using CleanArchitecture.Domain.Entities.Auths;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Persistence.Configurations;

public class AppUserConfiguration: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Identity_Users");

        builder.Property(user => user.FirstName)
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .HasMaxLength(100);
    }
}
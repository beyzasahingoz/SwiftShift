﻿using Bitirme.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bitirme.Models;
using Bitirme.Controllers;

namespace Bitirme.Areas.Identity.Data;

public class DbContextSwiftShift : IdentityDbContext<ApplicationUser>
{
    public DbContextSwiftShift(DbContextOptions<DbContextSwiftShift> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<District> District { get; set; }

    public virtual DbSet<ApplicationUser> AspNetUsers { get; set; }
    public DbSet<Explore> tbl_products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        builder.ApplyConfiguration(new ProductEntityConfiguration());
        //builder.ApplyConfiguration(new CountryEntityConfiguration());
        //builder.ApplyConfiguration(new CityEntityConfiguration());
    }
}

internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.Ad).HasMaxLength(255);
        builder.Property(x => x.Soyad).HasMaxLength(255);
        builder.Property(x => x.ProfilePicture);
        builder.Property(x => x.CountryId);
        builder.Property(x => x.CityId);
    }
}
internal class ProductEntityConfiguration : IEntityTypeConfiguration<Adverts>
{
    public void Configure(EntityTypeBuilder<Adverts> builder)
    {
        builder.Property(x => x.ProductName).HasMaxLength(255);
        builder.Property(x => x.Country).HasMaxLength(255);
        builder.Property(x => x.City).HasMaxLength(255);
        builder.Property(x => x.Address).HasMaxLength(255);
        builder.Property(x => x.ProductKg).HasMaxLength(255);
        builder.Property(x => x.Note).HasMaxLength(255);
    }
}

internal class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
       
    }
}

internal class CityEntityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
       
    }
}
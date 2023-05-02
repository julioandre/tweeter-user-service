using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.Models;

namespace user_service.Data;

public class ApplicationDbContext:IdentityDbContext<UserEntity>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserEntityConfiguration());
    }

    public DbSet<UserEntity> Users{get;set;}
}

public class UserEntityConfiguration:IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(150);
        builder.Property(x => x.LastName).HasMaxLength(150);
    }
}

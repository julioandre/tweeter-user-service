using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using user_service.Models;

namespace user_service.Data;

public class ApplicationDbContext:IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<UserEntity> Users{get;set;}
}
using AuthServer.Roles;
using AuthServer.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthServer;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");
        }
    }
}
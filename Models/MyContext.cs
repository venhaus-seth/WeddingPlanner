#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace WeddingPlanner.Models; // fill in ProjectName
public class MyContext : DbContext
{
    public MyContext(DbContextOptions options) : base(options) { }
    // create the following line for every model
    public DbSet<User> Users { get; set; } 
    public DbSet<Wedding> Weddings { get; set; } 
    public DbSet<Connection> Connections { get; set; } 
}
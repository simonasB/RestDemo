using DemoRest2024Live.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoRest2024Live.Data;

public class ForumDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    public ForumDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(("PostgreSQL")));
    }
}
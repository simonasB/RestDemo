using DemoRest20232.Auth.Model;
using DemoRest20232.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoRest20232.Data;

public class ForumDbContext : IdentityDbContext<ForumRestUser>
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
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}
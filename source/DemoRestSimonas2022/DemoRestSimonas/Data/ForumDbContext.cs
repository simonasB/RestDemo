using DemoRestSimonas.Auth.Model;
using DemoRestSimonas.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoRestSimonas.Data;

#nullable disable
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
        optionsBuilder.UseNpgsql(_configuration.GetValue<string>("PostgreSQLConnectionString"));
    }
}
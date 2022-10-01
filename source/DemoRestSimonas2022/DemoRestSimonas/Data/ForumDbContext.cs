using DemoRestSimonas.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoRestSimonas.Data;

public class ForumDbContext : DbContext
{
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=ForumDb2");
    }
}
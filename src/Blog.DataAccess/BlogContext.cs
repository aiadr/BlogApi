using Blog.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.DataAccess;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Post>();
    }
}
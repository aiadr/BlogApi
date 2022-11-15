using Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

internal class BlogContext : DbContext
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
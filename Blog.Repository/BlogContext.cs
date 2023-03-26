using Blog.Repository.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repository;

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
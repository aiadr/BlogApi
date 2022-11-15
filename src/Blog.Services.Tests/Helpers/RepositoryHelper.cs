using Blog.Infrastructure;
using Blog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Services.Tests.Helpers;

public static class RepositoryHelper
{
    public static IRepository<TEntity> BuildInMemoryRepository<TEntity>() where TEntity : class
    {
        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new Repository<TEntity>(new BlogContext(options));
    }
}
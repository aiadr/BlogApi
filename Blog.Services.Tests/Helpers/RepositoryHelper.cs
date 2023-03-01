using Blog.Infrastructure;
using Blog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Services.Tests.Helpers;

public static class RepositoryHelper
{
    public static IBlogRepository BuildInMemoryRepository()
    {
        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new BlogRepository(new BlogContext(options));
    }
}
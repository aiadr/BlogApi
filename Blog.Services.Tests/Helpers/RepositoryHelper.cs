using Blog.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Services.Tests.Helpers;

internal static class RepositoryHelper
{
    public static BlogContext BuildInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new BlogContext(options);
    }
}
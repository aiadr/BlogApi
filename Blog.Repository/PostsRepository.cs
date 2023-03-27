using System.Linq.Expressions;
using Blog.Repository.Contracts;
using Blog.Repository.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repository;

internal class PostsRepository : IPostsRepository
{
    private const int MaxPostsQueryLimit = 10000;

    private readonly DbContext _context;

    public PostsRepository(BlogContext context)
    {
        _context = context;
    }

    public Task<Post?> GetPostAsync(long id, CancellationToken cancellationToken)
    {
        return _context.Set<Post>().AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public IAsyncEnumerable<Post> GetAllPostsAsync(Expression<Func<Post, object>>? sortKeySelector,
        bool? sortDescending, int? limit, int? offset)
    {
        return BuildPostsQuery(_context.Set<Post>().AsNoTracking(), sortKeySelector, sortDescending, limit, offset)
            .AsAsyncEnumerable();
    }

    public async Task<Post> CreatePostAsync(Post post, CancellationToken cancellationToken)
    {
        await _context.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return post;
    }

    public async Task<bool> UpdatePostAsync(Post post, CancellationToken cancellationToken)
    {
        var changedCount = await _context.Set<Post>().Where(x => x.Id == post.Id).ExecuteUpdateAsync(
            x => x.SetProperty(y => y.Title, post.Title)
                .SetProperty(y => y.Content, post.Content)
                .SetProperty(y => y.UpdateDate, post.UpdateDate),
            cancellationToken: cancellationToken);

        return changedCount > 0;
    }

    public async Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken = default)
    {
        var changedCount = await _context.Set<Post>().Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
        return changedCount > 0;
    }

    private static IQueryable<Post> BuildPostsQuery(
        IQueryable<Post> queryable,
        Expression<Func<Post, object>>? sortFieldSelector,
        bool? sortDescending,
        int? limit,
        int? offset)
    {
        var query = queryable;

        if (sortFieldSelector != null)
        {
            if (sortDescending ?? false)
            {
                query = query.OrderByDescending(sortFieldSelector);
            }
            else
            {
                query = query.OrderBy(sortFieldSelector);
            }
        }

        if (offset > 0)
        {
            query = query.Skip(offset.Value);
        }

        if (limit == null || limit < 1 || limit > MaxPostsQueryLimit)
        {
            limit = MaxPostsQueryLimit;
        }

        query = query.Take(limit.Value);

        return query;
    }
}
using Blog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly BlogContext _context;

    public Repository(BlogContext context)
    {
        _context = context;
    }

    public Task<TEntity?> GetAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        CancellationToken cancellationToken = default)
    {
        return queryBuilder(_context.Set<TEntity>()).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        CancellationToken cancellationToken = default)
    {
        return queryBuilder(_context.Set<TEntity>()).ToListAsync(cancellationToken);
    }

    public Task<TResult?> GetReadOnlyAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken = default)
    {
        return queryBuilder(_context.Set<TEntity>().AsNoTracking()).FirstOrDefaultAsync(cancellationToken);
    }

    public IAsyncEnumerable<TResult> GetReadOnlyListAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder)
    {
        return queryBuilder(_context.Set<TEntity>()).AsAsyncEnumerable();
    }

    public async Task AddAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(item, cancellationToken);
    }

    public void Remove(TEntity item)
    {
        _context.Remove(item);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
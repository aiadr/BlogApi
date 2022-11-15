namespace Blog.Infrastructure.Contracts;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        CancellationToken cancellationToken = default);

    Task<TResult?> GetReadOnlyAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<TResult> GetReadOnlyListAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder);

    Task AddAsync(TEntity item, CancellationToken cancellationToken = default);
    
    void Remove(TEntity item);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
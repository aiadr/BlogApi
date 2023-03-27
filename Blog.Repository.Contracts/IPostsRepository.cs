using System.Linq.Expressions;
using Blog.Repository.Contracts.Models;

namespace Blog.Repository.Contracts;

public interface IPostsRepository
{
    Task<Post?> GetPostAsync(long id, CancellationToken cancellationToken);
    IAsyncEnumerable<Post> GetAllPostsAsync(
        Expression<Func<Post, object>>? sortKeySelector,
        bool? sortDescending,
        int? limit,
        int? offset);
    Task<Post> CreatePostAsync(Post post, CancellationToken cancellationToken);
    Task<bool> UpdatePostAsync(Post post, CancellationToken cancellationToken);
    Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken = default);
}
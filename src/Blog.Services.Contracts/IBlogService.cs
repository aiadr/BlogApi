using Blog.Entities;

namespace Blog.Services.Contracts;

public interface IBlogService
{
    Task<Post?> GetPostAsync(long id, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Post> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken = default);

    Task<Post> CreatePostAsync(Post post, CancellationToken cancellationToken = default);
    Task<Post?> UpdatePostAsync(Post post, CancellationToken cancellationToken = default);
    Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken = default);
}
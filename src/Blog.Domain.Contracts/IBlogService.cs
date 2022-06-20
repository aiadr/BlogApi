namespace Blog.Domain.Contracts;

public interface IBlogService
{
    Task<PostDto?> GetPostAsync(long id, CancellationToken cancellationToken = default);

    IAsyncEnumerable<PostDto> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken = default);

    Task<PostDto> CreatePostAsync(PostContentDto contentDto, CancellationToken cancellationToken = default);
    Task<PostDto?> UpdatePostAsync(long id, PostContentDto contentDto, CancellationToken cancellationToken = default);
    Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken = default);
}
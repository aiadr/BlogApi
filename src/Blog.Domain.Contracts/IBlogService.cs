namespace Blog.Domain.Contracts;

public interface IBlogService
{
    Task<PostDto?> GetPostAsync(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<PostDto> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken);

    Task<PostDto> CreatePostAsync(PostContentDto contentDto, CancellationToken cancellationToken);
    Task<PostDto?> UpdatePostAsync(long id, PostContentDto contentDto, CancellationToken cancellationToken);
    Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken);
}
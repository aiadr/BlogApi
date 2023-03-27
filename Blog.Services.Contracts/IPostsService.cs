using Blog.Services.Contracts.Models;

namespace Blog.Services.Contracts;

public interface IPostsService
{
    Task<PostDto?> GetPostAsync(long id, CancellationToken cancellationToken = default);

    IAsyncEnumerable<PostDto> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken = default);

    Task<PostDto> CreatePostAsync(EditPostDto postDto, CancellationToken cancellationToken = default);
    Task<bool> UpdatePostAsync(long id, EditPostDto postDto, CancellationToken cancellationToken = default);
    Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken = default);
}
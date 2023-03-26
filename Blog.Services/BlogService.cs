using System.Linq.Expressions;
using AutoMapper;
using Blog.Repository.Contracts.Models;
using Blog.Repository.Contracts.Services;
using Blog.Services.Contracts;

namespace Blog.Services;

internal class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;

    public BlogService(IBlogRepository context, IMapper mapper)
    {
        _blogRepository = context;
        _mapper = mapper;
    }

    public Task<Post?> GetPostAsync(long id, CancellationToken cancellationToken)
        => _blogRepository.GetPostAsync(id, cancellationToken);


    public IAsyncEnumerable<Post> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var sortFieldSelector = sortField == null ? null : GetSortKeySelector(sortField.Value);
        return _blogRepository.GetAllPostsAsync(sortFieldSelector, sortDescending, limit, offset);
    }


    public Task<Post> CreatePostAsync(Post post, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;
        post.CreationDate = currentDate;
        post.UpdateDate = currentDate;

        return _blogRepository.CreatePostAsync(post, cancellationToken);
    }

    public Task<bool> UpdatePostAsync(Post post, CancellationToken cancellationToken)
    {
        post.UpdateDate = DateTime.UtcNow;

        return _blogRepository.UpdatePostAsync(post, cancellationToken);
    }

    public Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken)
    {
        return _blogRepository.DeletePostAsync(id, cancellationToken);
    }

    private static Expression<Func<Post, object>> GetSortKeySelector(PostsSortField sortField)
    {
        return sortField switch
        {
            PostsSortField.Id => x => x.Id,
            PostsSortField.Title => x => x.Title,
            PostsSortField.CreationDate => x => x.CreationDate,
            PostsSortField.UpdateDate => x => x.UpdateDate,
            _ => throw new NotSupportedException(sortField.ToString())
        };
    }
}
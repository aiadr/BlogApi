using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMapper;
using Blog.Repository.Contracts;
using Blog.Repository.Contracts.Models;
using Blog.Services.Contracts;
using Blog.Services.Contracts.Models;

namespace Blog.Services;

internal class PostsService : IPostsService
{
    private readonly IPostsRepository _postsRepository;
    private readonly IMapper _mapper;

    public PostsService(IPostsRepository postsRepository, IMapper mapper)
    {
        _postsRepository = postsRepository;
        _mapper = mapper;
    }

    public async Task<PostDto?> GetPostAsync(long id, CancellationToken cancellationToken)
    {
        var post = await _postsRepository.GetPostAsync(id, cancellationToken);
        return _mapper.Map<PostDto?>(post);
    }

    public async IAsyncEnumerable<PostDto> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sortFieldSelector = sortField == null ? null : GetSortKeySelector(sortField.Value);
        await foreach (var post in _postsRepository.GetAllPostsAsync(sortFieldSelector, sortDescending, limit, offset)
                           .WithCancellation(cancellationToken))
        {
            yield return _mapper.Map<PostDto>(post);
        }
    }


    public async Task<PostDto> CreatePostAsync(EditPostDto postDto, CancellationToken cancellationToken)
    {
        var post = _mapper.Map<Post>(postDto);
        var currentDate = DateTime.UtcNow;
        post.CreationDate = currentDate;
        post.UpdateDate = currentDate;

        return _mapper.Map<PostDto>(await _postsRepository.CreatePostAsync(post, cancellationToken));
    }

    public Task<bool> UpdatePostAsync(long id, EditPostDto postDto, CancellationToken cancellationToken)
    {
        var post = _mapper.Map<Post>(postDto);
        post.Id = id;
        post.UpdateDate = DateTime.UtcNow;

        return _postsRepository.UpdatePostAsync(post, cancellationToken);
    }

    public Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken)
    {
        return _postsRepository.DeletePostAsync(id, cancellationToken);
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
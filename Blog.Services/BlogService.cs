using System.Linq.Expressions;
using AutoMapper;
using Blog.Entities;
using Blog.Infrastructure.Contracts;
using Blog.Services.Contracts;

namespace Blog.Services;

internal class BlogService : IBlogService
{
    private const int MaxPostsQueryLimit = 10000;

    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;

    public BlogService(IBlogRepository context, IMapper mapper)
    {
        _blogRepository = context;
        _mapper = mapper;
    }

    public Task<Post?> GetPostAsync(long id, CancellationToken cancellationToken)
        => _blogRepository.GetReadOnlyAsync<Post, Post>(query => query.Where(x => x.Id == id), cancellationToken);


    public IAsyncEnumerable<Post> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
        => _blogRepository.GetReadOnlyListAsync<Post, Post>(query =>
            BuildPostsQuery(query, sortField, sortDescending, limit, offset));


    public async Task<Post> CreatePostAsync(Post post, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;
        post.CreationDate = currentDate;
        post.UpdateDate = currentDate;

        await _blogRepository.AddAsync(post, cancellationToken);
        await _blogRepository.SaveChangesAsync(cancellationToken);

        return post;
    }

    public async Task<Post?> UpdatePostAsync(Post post, CancellationToken cancellationToken)
    {
        var savedPost =
            await _blogRepository.GetAsync<Post>(query => query.Where(x => x.Id == post.Id), cancellationToken);

        if (savedPost == null)
        {
            return null;
        }

        _mapper.Map(post, savedPost);

        savedPost.UpdateDate = DateTime.UtcNow;

        await _blogRepository.SaveChangesAsync(cancellationToken);

        return savedPost;
    }

    public async Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken)
    {
        var post = await _blogRepository.GetAsync<Post>(query => query.Where(x => x.Id == id), cancellationToken);

        if (post == null)
        {
            return false;
        }

        _blogRepository.Remove(post);
        await _blogRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static IQueryable<Post> BuildPostsQuery(
        IQueryable<Post> queryable,
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset)
    {
        var query = queryable;

        if (sortField != null)
        {
            var sortKeySelector = GetSortKeySelector(sortField.Value);
            if (sortDescending ?? false)
            {
                query = query.OrderByDescending(sortKeySelector);
            }
            else
            {
                query = query.OrderBy(sortKeySelector);
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
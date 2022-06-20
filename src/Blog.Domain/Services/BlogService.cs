using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMapper;
using Blog.DataAccess;
using Blog.DataAccess.Models;
using Blog.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Blog.Domain.Services;

internal class BlogService : IBlogService
{
    private const int MaxPostsQueryLimit = 10000;

    private readonly BlogContext _context;
    private readonly IMapper _mapper;

    public BlogService(BlogContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PostDto?> GetPostAsync(long id, CancellationToken cancellationToken)
    {
        var post = await _context.Set<Post>().AsNoTracking().Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            return null;
        }

        return _mapper.Map<PostDto>(post);
    }

    public async IAsyncEnumerable<PostDto> GetAllPostsAsync(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = BuildPostsQuery(_context, sortField, sortDescending, limit, offset);

        await foreach (var post in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return _mapper.Map<PostDto>(post);
        }
    }

    public async Task<PostDto> CreatePostAsync(PostContentDto contentDto, CancellationToken cancellationToken)
    {
        var post = _mapper.Map<Post>(contentDto);

        var currentDate = DateTime.UtcNow;
        post.CreationDate = currentDate;
        post.UpdateDate = currentDate;

        await _context.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PostDto>(post);
    }

    public async Task<PostDto?> UpdatePostAsync(long id, PostContentDto contentDto, CancellationToken cancellationToken)
    {
        var post = await _context.Set<Post>().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            return null;
        }

        _mapper.Map(contentDto, post);

        post.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PostDto>(post);
    }

    public async Task<bool> DeletePostAsync(long id, CancellationToken cancellationToken)
    {
        var post = await _context.Set<Post>().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            return false;
        }

        _context.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private static IQueryable<Post> BuildPostsQuery(
        DbContext context,
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset)
    {
        var query = context.Set<Post>().AsNoTracking();

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
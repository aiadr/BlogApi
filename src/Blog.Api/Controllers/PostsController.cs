using Blog.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : Controller
{
    private readonly IBlogService _blogService;

    public PostsController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(long id, CancellationToken cancellationToken)
    {
        var post = await _blogService.GetPostAsync(id, cancellationToken);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<PostDto>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<PostDto> GetAllPosts(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        CancellationToken cancellationToken) =>
        _blogService.GetAllPostsAsync(sortField, sortDescending, limit, offset, cancellationToken);

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePost(PostContentDto contentDto, CancellationToken cancellationToken)
    {
        return Ok(await _blogService.CreatePostAsync(contentDto, cancellationToken));
    }

    [Authorize]
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(long id, PostContentDto contentDto, CancellationToken cancellationToken)
    {
        var post = await _blogService.UpdatePostAsync(id, contentDto, cancellationToken);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(long id, CancellationToken cancellationToken)
    {
        if (await _blogService.DeletePostAsync(id, cancellationToken))
        {
            return Ok();
        }

        return NotFound();
    }
}
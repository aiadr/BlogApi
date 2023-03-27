using Blog.Services.Contracts;
using Blog.Services.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : Controller
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(long id, CancellationToken cancellationToken)
    {
        var post = await _postsService.GetPostAsync(id, cancellationToken);

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
        CancellationToken cancellationToken)
    {
        return _postsService.GetAllPostsAsync(
            sortField,
            sortDescending,
            limit,
            offset,
            cancellationToken);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePost(EditPostDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _postsService.CreatePostAsync(dto, cancellationToken));
    }

    [Authorize]
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(long id, EditPostDto dto, CancellationToken cancellationToken)
    {
        var success = await _postsService.UpdatePostAsync(id, dto, cancellationToken);

        return success ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(long id, CancellationToken cancellationToken)
    {
        if (await _postsService.DeletePostAsync(id, cancellationToken))
        {
            return Ok();
        }

        return NotFound();
    }
}
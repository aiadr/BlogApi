using System.Runtime.CompilerServices;
using AutoMapper;
using Blog.Api.Models;
using Blog.Repository.Contracts.Models;
using Blog.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : Controller
{
    private readonly IBlogService _blogService;
    private readonly IMapper _mapper;

    public PostsController(IBlogService blogService, IMapper mapper)
    {
        _blogService = blogService;
        _mapper = mapper;
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
    public async IAsyncEnumerable<PostDto> GetAllPosts(
        PostsSortField? sortField,
        bool? sortDescending,
        int? limit,
        int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var post in _blogService.GetAllPostsAsync(
                           sortField,
                           sortDescending,
                           limit,
                           offset,
                           cancellationToken))
        {
            yield return _mapper.Map<PostDto>(post);
        }
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePost(CreatePostDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _blogService.CreatePostAsync(_mapper.Map<Post>(dto), cancellationToken));
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(UpdatePostDto dto, CancellationToken cancellationToken)
    {
        var success = await _blogService.UpdatePostAsync(_mapper.Map<Post>(dto), cancellationToken);

        return success ? Ok() : NotFound();
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
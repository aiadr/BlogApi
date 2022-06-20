using AutoMapper;
using Blog.DataAccess;
using Blog.DataAccess.Models;
using Blog.Domain.Contracts;
using Blog.Domain.Services;
using Blog.Domain.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blog.Domain.Tests;

public class BlogServiceTests : IDisposable
{
    private const string TitleSample1 = "title1";
    private const string TitleSample2 = "title2";
    private const string TitleSample3 = "title3";
    private const string TitleSample4 = "title4";
    private const string ContentSample1 = "content1";
    private const string ContentSample2 = "content2";
    private const string ContentSample3 = "content3";
    private const string ContentSample4 = "content4";
    private const long IdSample1 = 1;

    private readonly BlogContext _dbContext = DbContextHelper.Build();
    private readonly IMapper _mapper = MapperHelper.Build();
    private readonly IBlogService _service;

    public BlogServiceTests()
    {
        _service = new BlogService(_dbContext, _mapper);
    }

    [Fact]
    public async Task GetPostAsync_Existing_Returns()
    {
        var post = new Post()
        {
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = DateTime.Now,
            UpdateDate = DateTime.Now
        };

        await _dbContext.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        var postDto = await _service.GetPostAsync(post.Id);

        postDto.Should().NotBeNull();
        postDto!.Id.Should().Be(post.Id);
        postDto.Title.Should().Be(post.Title);
        postDto.Content.Should().Be(post.Content);
        postDto.CreationDate.Should().Be(post.CreationDate);
        postDto.UpdateDate.Should().Be(post.UpdateDate);
    }

    [Fact]
    public async Task GetPostAsync_NotExisting_NotReturns()
    {
        var postDto = await _service.GetPostAsync(IdSample1);

        postDto.Should().BeNull();
    }

    [Fact]
    public async Task GetAllPostsAsync_Success()
    {
        var dateTime1 = DateTime.UtcNow;
        var dateTime2 = dateTime1.AddHours(1);
        var dateTime3 = dateTime1.AddHours(2);
        var dateTime4 = dateTime1.AddHours(3);

        var post1 = new Post()
        {
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = dateTime1,
            UpdateDate = dateTime1
        };
        var post2 = new Post()
        {
            Title = TitleSample2,
            Content = ContentSample2,
            CreationDate = dateTime2,
            UpdateDate = dateTime2
        };
        var post3 = new Post()
        {
            Title = TitleSample3,
            Content = ContentSample3,
            CreationDate = dateTime3,
            UpdateDate = dateTime3
        };
        var post4 = new Post()
        {
            Title = TitleSample4,
            Content = ContentSample4,
            CreationDate = dateTime4,
            UpdateDate = dateTime4
        };

        await _dbContext.AddAsync(post1);
        await _dbContext.AddAsync(post2);
        await _dbContext.AddAsync(post3);
        await _dbContext.AddAsync(post4);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetAllPostsAsync(PostsSortField.CreationDate, true, 1, 2).ToListAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        var postDto = result[0];

        postDto.Should().NotBeNull();
        postDto.Id.Should().Be(post2.Id);
        postDto.Title.Should().Be(post2.Title);
        postDto.Content.Should().Be(post2.Content);
        postDto.CreationDate.Should().Be(post2.CreationDate);
        postDto.UpdateDate.Should().Be(post2.UpdateDate);
        
    }

    [Fact]
    public async Task CreatePostAsync_Success()
    {
        var postContentDto = new PostContentDto(TitleSample1, ContentSample1);
        var postDto = await _service.CreatePostAsync(postContentDto);
        var post = await _dbContext.Set<Post>().Where(x => x.Id == postDto.Id).FirstOrDefaultAsync();

        post.Should().NotBeNull();
        post!.Id.Should().Be(postDto.Id);
        post.Title.Should().Be(postContentDto.Title);
        post.Content.Should().Be(postContentDto.Content);
        post.CreationDate.Should().Be(postDto.CreationDate);
        post.UpdateDate.Should().Be(postDto.UpdateDate);
    }

    [Fact]
    public async Task UpdatePostAsync_Existing_Success()
    {
        var creationDate = DateTime.UtcNow;
        var post = new Post()
        {
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = creationDate,
            UpdateDate = creationDate
        };

        await _dbContext.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        var postContentDto = new PostContentDto(TitleSample2, ContentSample2);

        var postDto = await _service.UpdatePostAsync(post.Id, postContentDto);

        var dateAfterUpdate = DateTime.UtcNow;

        postDto.Should().NotBeNull();
        postDto!.Id.Should().Be(post.Id);
        postDto.Title.Should().Be(TitleSample2);
        postDto.Content.Should().Be(ContentSample2);
        postDto.CreationDate.Should().Be(creationDate);
        postDto.UpdateDate.Should().BeAfter(creationDate);
        postDto.UpdateDate.Should().BeBefore(dateAfterUpdate);

        var updatedPost = await _dbContext.Set<Post>().Where(x => x.Id == post.Id).FirstOrDefaultAsync();

        updatedPost.Should().NotBeNull();
        updatedPost!.Id.Should().Be(post.Id);
        updatedPost.Title.Should().Be(TitleSample2);
        updatedPost.Content.Should().Be(ContentSample2);
        updatedPost.CreationDate.Should().Be(creationDate);
        updatedPost.UpdateDate.Should().BeAfter(creationDate);
        updatedPost.UpdateDate.Should().BeBefore(dateAfterUpdate);
        updatedPost.UpdateDate.Should().Be(postDto.UpdateDate);
    }

    [Fact]
    public async Task UpdatePostAsync_NotExisting_Failure()
    {
        var postContentDto = new PostContentDto(TitleSample1, ContentSample1);

        var postDto = await _service.UpdatePostAsync(IdSample1, postContentDto);

        postDto.Should().BeNull();

        var updatedPost = await _dbContext.Set<Post>().Where(x => x.Id == IdSample1).FirstOrDefaultAsync();

        updatedPost.Should().BeNull();
    }

    [Fact]
    public async Task DeletePostAsync_Existing_Success()
    {
        var creationDate = DateTime.UtcNow;
        var post = new Post()
        {
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = creationDate,
            UpdateDate = creationDate
        };

        await _dbContext.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        var result = await _service.DeletePostAsync(post.Id);

        result.Should().BeTrue();
        
        var deletedPost = await _dbContext.Set<Post>().Where(x => x.Id == post.Id).FirstOrDefaultAsync();

        deletedPost.Should().BeNull();
    }

    [Fact]
    public async Task DeletePostAsync_NotExisting_Failure()
    {
        var result = await _service.DeletePostAsync(IdSample1);

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
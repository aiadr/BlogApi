using Blog.Entities;
using Blog.Infrastructure.Contracts;
using Blog.Services.Contracts;
using Blog.Services.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Blog.Services.Tests;

public class BlogServiceTests
{
    private const string TitleSample1 = "title1";
    private const string TitleSample2 = "title2";
    private const string TitleSample3 = "title3";
    private const string ContentSample1 = "content1";
    private const string ContentSample2 = "content2";
    private const string ContentSample3 = "content3";
    private const long IdSample1 = 1;

    private readonly IBlogRepository _blogRepository;
    private readonly IBlogService _service;

    public BlogServiceTests()
    {
        _blogRepository = RepositoryHelper.BuildInMemoryRepository();
        _service = new BlogService(_blogRepository, MapperHelper.BuildMapper());
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

        await _blogRepository.AddAsync(post);
        await _blogRepository.SaveChangesAsync();

        var resultPost = await _service.GetPostAsync(post.Id);

        resultPost.Should().NotBeNull();
        resultPost!.Id.Should().Be(post.Id);
        resultPost.Title.Should().Be(post.Title);
        resultPost.Content.Should().Be(post.Content);
        resultPost.CreationDate.Should().Be(post.CreationDate);
        resultPost.UpdateDate.Should().Be(post.UpdateDate);
    }

    [Fact]
    public async Task GetPostAsync_NotExisting_NotReturns()
    {
        var resultPost = await _service.GetPostAsync(IdSample1);

        resultPost.Should().BeNull();
    }

    [Fact]
    public async Task GetAllPostsAsync_Success()
    {
        var dateTime1 = DateTime.UtcNow;
        var dateTime2 = dateTime1.AddHours(1);
        var dateTime3 = dateTime1.AddHours(2);

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

        await _blogRepository.AddAsync(post1);
        await _blogRepository.AddAsync(post2);
        await _blogRepository.AddAsync(post3);
        await _blogRepository.SaveChangesAsync();

        var result = await _service.GetAllPostsAsync(PostsSortField.CreationDate, true, 1, 1).ToListAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        var resultPost = result[0];

        resultPost.Should().NotBeNull();
        resultPost.Id.Should().Be(post2.Id);
        resultPost.Title.Should().Be(post2.Title);
        resultPost.Content.Should().Be(post2.Content);
        resultPost.CreationDate.Should().Be(post2.CreationDate);
        resultPost.UpdateDate.Should().Be(post2.UpdateDate);
    }

    [Fact]
    public async Task CreatePostAsync_Success()
    {
        var sourcePost = new Post
        {
            Title = TitleSample1,
            Content = ContentSample1
        };
        var resultPost = await _service.CreatePostAsync(sourcePost);
        var savedPost =
            await _blogRepository.GetReadOnlyAsync<Post, Post>(query => query.Where(x => x.Id == resultPost.Id));

        savedPost.Should().NotBeNull();
        savedPost!.Id.Should().Be(resultPost.Id);
        savedPost.Title.Should().Be(sourcePost.Title);
        savedPost.Content.Should().Be(sourcePost.Content);
        savedPost.CreationDate.Should().Be(resultPost.CreationDate);
        savedPost.UpdateDate.Should().Be(resultPost.UpdateDate);
    }

    [Fact]
    public async Task UpdatePostAsync_Existing_Success()
    {
        var creationDate = DateTime.UtcNow;
        var savedPost = new Post()
        {
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = creationDate,
            UpdateDate = creationDate
        };

        await _blogRepository.AddAsync(savedPost);
        await _blogRepository.SaveChangesAsync();

        var sourcePost = new Post
        {
            Id = savedPost.Id,
            Title = TitleSample2,
            Content = ContentSample2
        };

        var resultPost = await _service.UpdatePostAsync(sourcePost);

        var dateAfterUpdate = DateTime.UtcNow;

        resultPost.Should().NotBeNull();
        resultPost!.Id.Should().Be(savedPost.Id);
        resultPost.Title.Should().Be(TitleSample2);
        resultPost.Content.Should().Be(ContentSample2);
        resultPost.CreationDate.Should().Be(creationDate);
        resultPost.UpdateDate.Should().BeAfter(creationDate);
        resultPost.UpdateDate.Should().BeBefore(dateAfterUpdate);

        var updatedPost =
            await _blogRepository.GetReadOnlyAsync<Post, Post>(query => query.Where(x => x.Id == savedPost.Id));

        updatedPost.Should().NotBeNull();
        updatedPost!.Id.Should().Be(savedPost.Id);
        updatedPost.Title.Should().Be(TitleSample2);
        updatedPost.Content.Should().Be(ContentSample2);
        updatedPost.CreationDate.Should().Be(creationDate);
        updatedPost.UpdateDate.Should().BeAfter(creationDate);
        updatedPost.UpdateDate.Should().BeBefore(dateAfterUpdate);
        updatedPost.UpdateDate.Should().Be(resultPost.UpdateDate);
    }

    [Fact]
    public async Task UpdatePostAsync_NotExisting_Failure()
    {
        var sourcePost = new Post
        {
            Id = IdSample1,
            Title = TitleSample1,
            Content = ContentSample1
        };

        var resultPost = await _service.UpdatePostAsync(sourcePost);

        resultPost.Should().BeNull();

        var updatedPost =
            await _blogRepository.GetReadOnlyAsync<Post, Post>(query => query.Where(x => x.Id == IdSample1));

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

        await _blogRepository.AddAsync(post);
        await _blogRepository.SaveChangesAsync();

        var result = await _service.DeletePostAsync(post.Id);

        result.Should().BeTrue();

        var deletedPost =
            await _blogRepository.GetReadOnlyAsync<Post, Post>(query => query.Where(x => x.Id == post.Id));

        deletedPost.Should().BeNull();
    }

    [Fact]
    public async Task DeletePostAsync_NotExisting_Failure()
    {
        var result = await _service.DeletePostAsync(IdSample1);

        result.Should().BeFalse();
    }
}
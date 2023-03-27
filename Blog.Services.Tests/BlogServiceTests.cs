using System.Linq.Expressions;
using Blog.Repository.Contracts;
using Blog.Repository.Contracts.Models;
using Blog.Services.Contracts;
using Blog.Services.Contracts.Models;
using Blog.Services.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blog.Services.Tests;

public class BlogServiceTests
{
    private const string TitleSample1 = "title1";
    private const string TitleSample2 = "title2";
    private const string ContentSample1 = "content1";
    private const string ContentSample2 = "content2";
    private const long IdSample1 = 1;
    private const long IdSample2 = 2;

    private readonly IPostsService _service;
    private readonly Mock<IPostsRepository> _repositoryMock;

    public BlogServiceTests()
    {
        _repositoryMock = new Mock<IPostsRepository>();

        _service = new PostsService(_repositoryMock.Object, MapperHelper.BuildMapper());
    }

    [Fact]
    public async Task GetPostAsync_Existing_Returns()
    {
        var post = new Post()
        {
            Id = IdSample1,
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = DateTime.Now,
            UpdateDate = DateTime.Now
        };

        _repositoryMock.Setup(x => x.GetPostAsync(It.Is<long>(y => y == IdSample1), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Post?)post));

        var resultPost = await _service.GetPostAsync(IdSample1);

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
    public async Task GetAllPostsAsync_Test()
    {
        var dateTime1 = DateTime.UtcNow;
        var dateTime2 = dateTime1.AddHours(1);

        var post1 = new Post()
        {
            Id = IdSample1,
            Title = TitleSample1,
            Content = ContentSample1,
            CreationDate = dateTime1,
            UpdateDate = dateTime1
        };
        var post2 = new Post()
        {
            Id = IdSample2,
            Title = TitleSample2,
            Content = ContentSample2,
            CreationDate = dateTime2,
            UpdateDate = dateTime2
        };

        const bool descending = true;
        const int limit = 123;
        const int offset = 456;

        _repositoryMock.Setup(x => x.GetAllPostsAsync(
                It.Is<Expression<Func<Post, object>>>(y =>
                    ((MemberExpression)((UnaryExpression)y.Body).Operand).Member.Name == nameof(Post.CreationDate)),
                It.Is<bool?>(y => y == descending),
                It.Is<int?>(y => y == limit),
                It.Is<int?>(y => y == offset)))
            .Returns(new[] { post1, post2 }.ToAsyncEnumerable());

        var result = await _service.GetAllPostsAsync(PostsSortField.CreationDate, descending, limit, offset)
            .ToListAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var result1 = result.Should().Contain(x => x.Id == post1.Id).Subject;

        result1.Should().NotBeNull();
        result1.Id.Should().Be(post1.Id);
        result1.Title.Should().Be(post1.Title);
        result1.Content.Should().Be(post1.Content);
        result1.CreationDate.Should().Be(post1.CreationDate);
        result1.UpdateDate.Should().Be(post1.UpdateDate);

        var result2 = result.Should().Contain(x => x.Id == post2.Id).Subject;

        result2.Should().NotBeNull();
        result2.Id.Should().Be(post2.Id);
        result2.Title.Should().Be(post2.Title);
        result2.Content.Should().Be(post2.Content);
        result2.CreationDate.Should().Be(post2.CreationDate);
        result2.UpdateDate.Should().Be(post2.UpdateDate);
    }

    [Fact]
    public async Task CreatePostAsync_Test()
    {
        var sourcePost = new EditPostDto(TitleSample1, ContentSample1);
        var dateBeforeCreation = DateTime.UtcNow;

        Post createdPost = null!;

        _repositoryMock.Setup(x => x.CreatePostAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .Callback<Post, CancellationToken>((post, _) =>
            {
                var dateAfterCreation = DateTime.UtcNow;

                post.Should().NotBeNull();
                post.Id.Should().Be(default);
                post.Title.Should().Be(TitleSample1);
                post.Content.Should().Be(ContentSample1);
                post.CreationDate.Should().BeAfter(dateBeforeCreation);
                post.CreationDate.Should().BeBefore(dateAfterCreation);
                post.UpdateDate.Should().Be(post.CreationDate);

                createdPost = post;
            })
            .Returns<Post, CancellationToken>((post, _) => Task.FromResult(post));

        var resultPost = await _service.CreatePostAsync(sourcePost);
        
        _repositoryMock.Verify(x => x.CreatePostAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
        
        resultPost.Should().BeEquivalentTo(createdPost);
    }

    [Fact]
    public async Task UpdatePostAsync_Test()
    {
        var editPostDto = new EditPostDto(TitleSample1, ContentSample1);

        var dateBeforeUpdate = DateTime.UtcNow;

        _repositoryMock.Setup(x => x.UpdatePostAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .Callback<Post, CancellationToken>((post, _) =>
            {
                var dateAfterUpdate = DateTime.UtcNow;

                post.Should().NotBeNull();
                post!.Id.Should().Be(IdSample1);
                post.Title.Should().Be(TitleSample1);
                post.Content.Should().Be(ContentSample1);
                post.UpdateDate.Should().BeAfter(dateBeforeUpdate);
                post.UpdateDate.Should().BeBefore(dateAfterUpdate);
            })
            .Returns(Task.FromResult(true));

        var result = await _service.UpdatePostAsync(IdSample1, editPostDto);

        result.Should().BeTrue();

        _repositoryMock.Verify(x => x.UpdatePostAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePostAsync_Test()
    {
        _repositoryMock.Setup(x => x.DeletePostAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        var result = await _service.DeletePostAsync(IdSample1);

        result.Should().BeTrue();

        _repositoryMock.Verify(x => x.DeletePostAsync(IdSample1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
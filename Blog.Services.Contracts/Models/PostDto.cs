namespace Blog.Services.Contracts.Models;

public record PostDto(long Id, string Title, string Content, DateTime CreationDate, DateTime UpdateDate);
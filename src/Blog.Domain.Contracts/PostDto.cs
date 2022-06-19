namespace Blog.Domain.Contracts;

public record PostDto(long Id, string Title, string Content, DateTime CreationDate, DateTime UpdateDate);
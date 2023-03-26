namespace Blog.Repository.Contracts.Models;

public class Post
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}
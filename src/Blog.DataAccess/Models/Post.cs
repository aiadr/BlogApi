using System.ComponentModel.DataAnnotations;

namespace Blog.DataAccess.Models;

public class Post
{
    public long Id { get; set; }
    
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    [Required]
    public DateTime CreationDate { get; set; }
    
    [Required]
    public DateTime UpdateDate { get; set; }
}
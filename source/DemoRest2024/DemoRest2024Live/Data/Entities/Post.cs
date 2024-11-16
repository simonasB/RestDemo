using System.ComponentModel.DataAnnotations;
using DemoRest2024Live.Auth.Model;

namespace DemoRest2024Live.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    
    public Topic Topic { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public ForumUser User { get; set; }
}
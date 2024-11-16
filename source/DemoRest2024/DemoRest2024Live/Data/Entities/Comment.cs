using System.ComponentModel.DataAnnotations;
using DemoRest2024Live.Auth.Model;

namespace DemoRest2024Live.Data.Entities;

public class Comment
{
    // Guid
    // Ulid
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    
    public Post Post { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public ForumUser User { get; set; }
}
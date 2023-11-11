using System.ComponentModel.DataAnnotations;
using DemoRest20232.Auth.Model;

namespace DemoRest20232.Data.Entities;

public class Topic
{
    // guid
    // ulid
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime CreationDate { get; set; }
    public DateTime? ExpiresIn { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public ForumRestUser User { get; set; }
}

public record TopicDto(int Id, string Name, string Description, DateTime CreationDate);
using System.ComponentModel.DataAnnotations;
using DemoRest20232.Auth.Model;

namespace DemoRest20232.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreationDate { get; set; }

    public required Post Post { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public ForumRestUser User { get; set; }
}
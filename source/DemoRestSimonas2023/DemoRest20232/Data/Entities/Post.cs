using System.ComponentModel.DataAnnotations;
using DemoRest20232.Auth.Model;

namespace DemoRest20232.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Body { get; set; }
    public DateTime CreationDate { get; set; }

    public required Topic Topic { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public ForumRestUser User { get; set; }
}
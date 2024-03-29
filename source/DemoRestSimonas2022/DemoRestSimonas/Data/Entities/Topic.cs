using System.ComponentModel.DataAnnotations;
using DemoRestSimonas.Auth.Model;

namespace DemoRestSimonas.Data.Entities;

public class Topic : IUserOwnedResource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    
    // Only can be set by admin
    public DateTime? ExpiresIn { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    public ForumRestUser User { get; set; }
}
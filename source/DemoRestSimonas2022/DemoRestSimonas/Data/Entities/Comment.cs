namespace DemoRestSimonas.Data.Entities;

public class Comment
{
    // Guid
    // Ulid
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }
    
    public Post Post { get; set; }
}
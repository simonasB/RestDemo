namespace DemoRest2024Live.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    
    public Topic Topic { get; set; }
}
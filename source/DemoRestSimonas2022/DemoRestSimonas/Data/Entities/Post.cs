namespace DemoRestSimonas.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Body { get; set; }
    public DateTime CreationDate { get; set; }
    
    public Topic Topic { get; set; }
}
using System;

namespace DemoRestSimonas.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public DateTime CreationDateUtc { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}

namespace DemoRestSimonas.Data.Dtos.Topics;

public record TopicDto(int Id, string Name, string Description, DateTime CreationDate);
public record CreateTopicDto(string Name, string Description);
public record UpdateTopicDto(string Description);
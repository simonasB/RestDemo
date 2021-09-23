using System.ComponentModel.DataAnnotations;

namespace DemoRestSimonas.Data.Dtos.Topics
{
    public record UpdateTopicDto([Required] string Name);
}

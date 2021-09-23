using System.ComponentModel.DataAnnotations;

namespace DemoRestSimonas.Data.Dtos.Topics
{
    public record CreateTopicDto([Required] string Name, [Required] string Description);
}

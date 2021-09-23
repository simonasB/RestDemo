using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoRestSimonas.Data.Dtos.Posts
{
    public record CreatePostDto([Required] string Name, [Required] string Body);
}

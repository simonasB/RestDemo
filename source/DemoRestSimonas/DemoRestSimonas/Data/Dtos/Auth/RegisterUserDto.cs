using System.ComponentModel.DataAnnotations;

namespace DemoRestSimonas.Data.Dtos.Auth
{
    public record RegisterUserDto([Required] string UserName, [EmailAddress][Required] string Email,
        [Required] string Password);
}

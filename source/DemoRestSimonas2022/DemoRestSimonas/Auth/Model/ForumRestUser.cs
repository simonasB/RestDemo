using Microsoft.AspNetCore.Identity;

namespace DemoRestSimonas.Auth.Model;

public class ForumRestUser : IdentityUser
{
    [PersonalData]
    public string? AdditionalInfo { get; set; }
}
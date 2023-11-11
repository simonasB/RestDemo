using Microsoft.AspNetCore.Identity;

namespace DemoRest20232.Auth.Model;

public class ForumRestUser : IdentityUser
{
    public bool ForceRelogin { get; set; }
}
namespace DemoRest20232.Auth.Model;

public static class ForumRoles
{
    public const string Admin = nameof(Admin);
    public const string ForumUser = nameof(ForumUser);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, ForumUser };
}
using System.Security.Cryptography;
using System.Text;

namespace DemoRest2024Live.Helpers;

public static class Extensions
{
    /// <summary>
    /// Salting
    /// HMACSHA256
    /// key-stretching algorithms like PBKDF2 or bcrypt
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToSHA256(this string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}
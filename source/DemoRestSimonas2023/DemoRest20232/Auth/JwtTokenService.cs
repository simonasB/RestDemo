using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace DemoRest20232.Auth;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _authSigningKey;
    private readonly string _issuer;
    private readonly string _audience;
    
    public JwtTokenService(IConfiguration configuration)
    {
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        _issuer = configuration["JWT:ValidIssuer"];
        _audience = configuration["JWT:ValidAudience"];
    }
    
    public string CreateAccessToken(string userName, string userId, IEnumerable<string> roles)
    {
        var authClaims = new List<Claim>()
        {
            new(ClaimTypes.Name, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId)
        };
        
        authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken
        (
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.UtcNow.AddMinutes(10),
            claims: authClaims,
            signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string CreateRefreshToken(string userId)
    {
        var authClaims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId)
        };

        var token = new JwtSecurityToken
        (
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.UtcNow.AddHours(24),
            claims: authClaims,
            signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool TryParseRefreshToken(string refreshToken, out ClaimsPrincipal? claims)
    {
        claims = null;
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = _authSigningKey,
                ValidateLifetime = true,
            };
            
            claims = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
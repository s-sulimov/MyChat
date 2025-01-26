using Microsoft.IdentityModel.Tokens;
using Sulimov.MyChat.Server.Core.Models.Responses;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sulimov.MyChat.Server.Authorization.Services;

/// <inheritdoc/>
public class JwtService : IJwtService
{
    private readonly IConfiguration configuration;
    private readonly int expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        this.configuration = configuration;
        expirationMinutes = configuration.GetSection("Jwt").GetValue<int>("ExpirationMinutes");
    }

    /// <inheritdoc/>
    public AuthenticationResponse CreateToken(string userId, string userName, string userEmail, IReadOnlyCollection<Claim> roleClaims)
    {
        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(userId, userName, userEmail, roleClaims),
            CreateSigningCredentials(),
            expiration
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return new AuthenticationResponse(tokenHandler.WriteToken(token), expiration);
    }

    private static Claim[] CreateClaims(string userId, string userName, string userEmail, IReadOnlyCollection<Claim> roleClaims)
    {
        var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString(new CultureInfo("en-US"))),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.NameIdentifier, userName),
            };

        foreach (Claim claim in roleClaims)
        {
            claims.Add(claim);
        }

        return claims.ToArray();
    }

    private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

    private SigningCredentials CreateSigningCredentials() =>
        new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
            ),
            SecurityAlgorithms.HmacSha256
        );
}

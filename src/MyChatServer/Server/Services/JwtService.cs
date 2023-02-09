using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sulimov.MyChat.Server.BL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sulimov.MyChat.Server.Services
{
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
        public AuthenticationResponse CreateToken(IdentityUser user, IEnumerable<Claim> roleClaims)
        {
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var token = CreateJwtToken(
                CreateClaims(user, roleClaims),
                CreateSigningCredentials(),
                expiration
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }

        private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private Claim[] CreateClaims(IdentityUser user, IEnumerable<Claim> roleClaims)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (Claim claim in roleClaims)
            {
                claims.Add(claim);
            }

            return claims.ToArray();
        }

        private SigningCredentials CreateSigningCredentials() =>
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                ),
                SecurityAlgorithms.HmacSha256
            );
    }
}

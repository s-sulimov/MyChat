using Microsoft.IdentityModel.Tokens;
using Sulimov.MyChat.Server.DAL.Models;
using Sulimov.MyChat.Server.Models.Responses;
using System.Globalization;
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
        public AuthenticationResponse CreateToken(DbUser user, IReadOnlyCollection<Claim> roleClaims)
        {
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var token = CreateJwtToken(
                CreateClaims(user, roleClaims),
                CreateSigningCredentials(),
                expiration
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AuthenticationResponse(tokenHandler.WriteToken(token), expiration);
        }

        private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private static Claim[] CreateClaims(DbUser user, IReadOnlyCollection<Claim> roleClaims)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString(new CultureInfo("en-US"))),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.UserName!),
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
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                ),
                SecurityAlgorithms.HmacSha256
            );
    }
}

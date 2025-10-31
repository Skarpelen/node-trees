using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NodeTrees.BusinessLogic.Helpers
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.Shared;

    public static class JwtHelper
    {
        public static string CreateAccessToken(
            User user,
            Guid jti,
            JwtAuthorizationConfiguration cfg,
            IEnumerable<Claim>? extraClaims = null,
            TimeSpan? lifetime = null)
        {
            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Code ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            if (extraClaims is not null)
            {
                claims = extraClaims.Aggregate(claims, (current, extraClaim) => current.Append(extraClaim).ToArray());
            }

            var expires = now.Add(lifetime ?? TimeSpan.FromSeconds(cfg.ExpirationSeconds));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: cfg.Issuer,
                audience: cfg.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

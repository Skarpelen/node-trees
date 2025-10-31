using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace NodeTrees.Authentication
{
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.BusinessLogic.Services;
    using NodeTrees.Shared;

    public static class Jwt
    {
        public static string AuthType => JwtBearerDefaults.AuthenticationScheme;
    }

    public static class JwtExtensions
    {
        public static void ConfigureJwtAuthentication(this WebApplicationBuilder builder)
        {
            var jwtConfig = builder.Configuration.GetSection("JwtAuthorization").Get<JwtAuthorizationConfiguration>();

            if (jwtConfig == null)
            {
                throw new InvalidOperationException("JwtAuthorization configuration is missing");
            }

            builder.Services.Configure<JwtAuthorizationConfiguration>(builder.Configuration.GetSection("JwtAuthorization"));

            builder.Services.AddAuthentication(Jwt.AuthType)
                .AddJwtBearer(opts =>
                {
                    opts.MapInboundClaims = false;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey))
                    };

                    opts.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var usr = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var jtiStr = ctx.Principal!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                            if (!Guid.TryParse(jtiStr, out var jti))
                            {
                                ctx.Fail("invalid jti");
                                return;
                            }

                            var sub = ctx.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                            var user = await usr.GetUserBySecurityStampAsync(sub!);

                            if (user is null || user.SecurityStamp != sub)
                            {
                                ctx.Fail("blocked or stamp mismatch");
                            }
                        },

                        OnMessageReceived = ctx =>
                        {
                            var token = ctx.Request.Headers["Authorization"]
                                .FirstOrDefault()?.Replace("Bearer ", "");

                            token ??= ctx.Request.Query["access_token"].FirstOrDefault();

                            var path = ctx.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(token))
                            {
                                ctx.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(Jwt.AuthType)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(JwtAuthPolicies.Admin,
                    p => p.RequireRole(UserRole.Admin.ToString()));

                options.AddPolicy(JwtAuthPolicies.User,
                    p => p.RequireRole(UserRole.Admin.ToString(), UserRole.User.ToString()));
            });
        }
    }
}

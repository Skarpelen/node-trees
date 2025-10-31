using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NodeTrees.WebApi.Context
{
    using NodeTrees.BusinessLogic.Context;

    public sealed class HttpCurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpCurrentUserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string? SecurityStamp
        {
            get
            {
                var http = _accessor.HttpContext;

                if (http == null)
                {
                    return null;
                }

                var user = http.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? user.FindFirst("sub")?.Value
                    ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("nameid")?.Value;

                return string.IsNullOrWhiteSpace(sub) ? null : sub;
            }
        }

        public string? IpAddress
        {
            get
            {
                var http = _accessor.HttpContext;
                return http?.Connection.RemoteIpAddress?.ToString();
            }
        }

        public string? UserAgent
        {
            get
            {
                var http = _accessor.HttpContext;
                return http?.Request.Headers.UserAgent.ToString();
            }
        }

        public bool IsAnonymous
        {
            get
            {
                var http = _accessor.HttpContext;
                return http?.User?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(SecurityStamp);
            }
        }

        public CancellationToken CancellationToken
        {
            get
            {
                var http = _accessor.HttpContext;
                return http?.RequestAborted ?? CancellationToken.None;
            }
        }
    }
}

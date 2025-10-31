using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace NodeTrees.WebApi.Context
{
    using NodeTrees.BusinessLogic.Context;

    public sealed class SnapshotCurrentUserContext : ICurrentUserContext
    {
        public SnapshotCurrentUserContext(HttpContext http)
        {
            var user = http.User;

            SecurityStamp =
                user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value;

            IpAddress = http.Connection.RemoteIpAddress?.ToString();

            UserAgent = http.Request.Headers.UserAgent.ToString();

            IsAnonymous = string.IsNullOrEmpty(SecurityStamp);

            CancellationToken = http.RequestAborted;
        }

        public string? SecurityStamp { get; }

        public string? IpAddress { get; }

        public string? UserAgent { get; }

        public bool IsAnonymous { get; }

        public CancellationToken CancellationToken { get; }
    }
}

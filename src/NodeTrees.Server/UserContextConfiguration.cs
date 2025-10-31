namespace NodeTrees.Server
{
    using NodeTrees.BusinessLogic.Context;
    using NodeTrees.WebApi.Context;

    internal static class UserContextConfiguration
    {
        public static void ConfigureUserContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
            //builder.Services.AddScoped<ICurrentUserContext>(sp =>
            //{
            //    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
            //    var http = accessor.HttpContext;

            //    if (http != null)
            //    {
            //        var feature = http.Features.Get<ICurrentUserContextFeature>();
            //        if (feature != null)
            //        {
            //            return feature.Context;
            //        }

            //        return new SnapshotCurrentUserContext(http);
            //    }

            //    return new AmbientEmptyUserContext();
            //});
        }

        private sealed class AmbientEmptyUserContext : ICurrentUserContext
        {
            public string? SecurityStamp => null;

            public string? IpAddress => null;

            public string? UserAgent => null;

            public bool IsAnonymous => true;

            public CancellationToken CancellationToken => CancellationToken.None;
        }
    }
}

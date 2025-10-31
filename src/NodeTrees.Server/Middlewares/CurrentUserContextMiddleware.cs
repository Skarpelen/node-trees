namespace NodeTrees.Server.Middlewares
{
    using NodeTrees.WebApi.Context;

    internal sealed class CurrentUserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext http)
        {
            var ctx = new SnapshotCurrentUserContext(http);

            http.Features.Set<ICurrentUserContextFeature>(new CurrentUserContextFeature(ctx));

            await _next(http);
        }
    }
}

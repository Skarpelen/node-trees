using NLog;
using System.Text;
using System.Text.Json;

namespace NodeTrees.Server.Middlewares
{
    using NodeTrees.BusinessLogic.Services;
    using NodeTrees.Shared;

    internal sealed class ExceptionHandlingMiddleware(IServiceProvider serviceProvider) : IMiddleware
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (ex is SecureException sec)
                {
                    var queryJson = BuildQueryJson(context);
                    var bodyJson = await TryReadBodyAsync(context);

                    sec.QueryParameters = queryJson;
                    sec.BodyParameters = bodyJson;

                    await HandleAsync(context, ex, sec);
                }
                else
                {
                    await HandleAsync(context, ex, null);
                }
            }
        }

        private static string BuildQueryJson(HttpContext context)
        {
            if (context.Request.Query.Count == 0)
            {
                return string.Empty;
            }

            var map = context.Request
                .Query
                .ToDictionary(kv => kv.Key, kv => (string?)kv.Value.ToString());

            var json = JsonSerializer.Serialize(map);
            return json;
        }

        private static async Task<string> TryReadBodyAsync(HttpContext context)
        {
            if (context.Request.ContentLength is not > 0)
            {
                return string.Empty;
            }

            if (!context.Request.Body.CanSeek)
            {
                context.Request.EnableBuffering();
            }

            context.Request.Body.Position = 0;

            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0;

            return body;
        }

        private async Task HandleAsync(HttpContext context, Exception exception, SecureException? sec)
        {
            var eventId = DateTime.UtcNow.Ticks;
            var status = StatusCodes.Status500InternalServerError;

            var httpMethod = context.Request.Method;
            var path = context.Request.Path.HasValue ? context.Request.Path.Value : null;

            var queryJson = sec?.QueryParameters ?? BuildQueryJson(context);
            var bodyJson = sec?.BodyParameters ?? await TryReadBodyAsync(context);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var journal = scope.ServiceProvider.GetRequiredService<IJournalService>();

                await journal.LogExceptionAsync(
                    eventId: eventId,
                    exception: exception,
                    httpMethod: httpMethod,
                    path: path,
                    queryJson: string.IsNullOrWhiteSpace(queryJson) ? null : queryJson,
                    bodyJson: string.IsNullOrWhiteSpace(bodyJson) ? null : bodyJson,
                    statusCode: status,
                    ct: CancellationToken.None);
            }
            catch (Exception dbEx)
            {
                _log.Error(dbEx, "Failed to log exception to DB. originalEventId={EventId}", eventId);
            }

            var isSecure = exception is SecureException secEx;
            var pubType = isSecure ? (exception as SecureException)!.PublicType : "Exception";
            var msg = isSecure
                ? exception.Message
                : $"Internal server error ID = {eventId}";

            var payload = new
            {
                type = pubType,
                id = eventId.ToString(),
                data = new { message = msg }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }
}

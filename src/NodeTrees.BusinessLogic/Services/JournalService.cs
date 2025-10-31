using NLog;
using System.Text;

namespace NodeTrees.BusinessLogic.Services
{
    using NodeTrees.BusinessLogic.Context;
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.Shared.DTO;

    public interface IJournalService
    {
        Task<Range<JournalEvent>> GetRangeAsync(int skip, int take, JournalFilterDto? filter);
        Task<JournalEvent?> GetSingleAsync(long id);
        Task<long> LogExceptionAsync(
           long eventId,
           Exception exception,
           string? httpMethod,
           string? path,
           string? queryJson,
           string? bodyJson,
           int statusCode,
           CancellationToken ct = default);
    }

    public sealed class JournalService(IUnitOfWork unitOfWork, ICurrentUserContext context) : IJournalService
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task<Range<JournalEvent>> GetRangeAsync(int skip, int take, JournalFilterDto? filter)
        {
            if (skip < 0)
            {
                skip = 0;
            }

            if (take <= 0)
            {
                take = 50;
            }

            var ct = context.CancellationToken;

            var list = await unitOfWork.JournalEvents.GetRangeAsync(
                skip,
                take,
                filter?.From,
                filter?.To,
                filter?.Search,
                ct);

            var result = new Range<JournalEvent>
            {
                Skip = skip,
                Count = list.Count,
                Items = list.ToList()
            };

            return result;
        }

        public async Task<JournalEvent?> GetSingleAsync(long id)
        {
            var ct = context.CancellationToken;
            var entity = await unitOfWork.JournalEvents.GetByIdAsync(id, ct);

            if (entity == null)
            {
                _log.Warn("Journal record not found id={Id}", id);
                return null;
            }

            return entity;
        }

        public async Task<long> LogExceptionAsync(
            long eventId,
            Exception exception,
            string? httpMethod,
            string? path,
            string? queryJson,
            string? bodyJson,
            int statusCode,
            CancellationToken ct = default)
        {
            var sb = new StringBuilder();
            var cur = exception;

            while (cur != null)
            {
                sb.AppendLine($"{cur.GetType().FullName}: {cur.Message}");
                sb.AppendLine(cur.StackTrace ?? "N/A");
                cur = cur.InnerException;
            }

            var ent = new JournalEvent
            {
                EventId = eventId,
                TimestampUtc = DateTime.UtcNow,
                ExceptionType = exception.GetType().FullName ?? "Exception",
                Message = exception.Message ?? string.Empty,
                StackTrace = sb.ToString(),
                HttpMethod = httpMethod,
                Path = path,
                QueryJson = string.IsNullOrWhiteSpace(queryJson) ? null : queryJson,
                BodyJson = string.IsNullOrWhiteSpace(bodyJson) ? null : bodyJson,
                StatusCode = statusCode
            };

            await unitOfWork.JournalEvents.Add(ent);
            await unitOfWork.CompleteAsync(ct);

            _log.Error("Logged exception eventId={EventId} path={Path} method={Method}", eventId, path, httpMethod);

            return eventId;
        }
    }
}

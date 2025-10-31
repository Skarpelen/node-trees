using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeTrees.BusinessLogic.Models
{
    [Table("exception_journal")]
    public sealed class JournalEvent : BaseModel
    {
        [Required]
        [Column("event_id")]
        public long EventId { get; set; }

        [Required]
        [Column("timestamp_utc")]
        public DateTime TimestampUtc { get; set; }

        [Required]
        [MaxLength(512)]
        [Column("exception_type")]
        public string ExceptionType { get; set; } = string.Empty;

        [Required]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("stack_trace", TypeName = "text")]
        public string? StackTrace { get; set; }

        [MaxLength(16)]
        [Column("http_method")]
        public string? HttpMethod { get; set; }

        [MaxLength(1024)]
        [Column("path")]
        public string? Path { get; set; }

        [Column("query_json")]
        public string? QueryJson { get; set; }

        [Column("body_json")]
        public string? BodyJson { get; set; }

        [Column("status_code")]
        public int? StatusCode { get; set; }
    }
}

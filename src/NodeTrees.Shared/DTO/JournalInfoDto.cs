using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace NodeTrees.Shared.DTO
{
    [SwaggerSchema(Description = "Lightweight journal entry info.")]
    public sealed class JournalInfoDto
    {
        [SwaggerSchema(Description = "Unique identifier of the journal entry.", Format = "int64")]
        [Required]
        public long Id { get; set; }

        [SwaggerSchema(Description = "Associated event identifier.", Format = "int64")]
        [Required]
        public long EventId { get; set; }

        [SwaggerSchema(Description = "Creation timestamp (UTC).", Format = "date-time")]
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

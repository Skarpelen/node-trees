using Swashbuckle.AspNetCore.Annotations;

namespace NodeTrees.Shared.DTO
{
    [SwaggerSchema(Description = "All fields are optional")]
    public sealed class JournalFilterDto
    {
        [SwaggerSchema(Description = "Inclusive lower bound for CreatedAt filter.", Format = "date-time")]
        public DateTime? From { get; set; }

        [SwaggerSchema(Description = "Inclusive upper bound for CreatedAt filter.", Format = "date-time")]
        public DateTime? To { get; set; }

        [SwaggerSchema(Description = "Free-text search query.")]
        public string? Search { get; set; }
    }
}

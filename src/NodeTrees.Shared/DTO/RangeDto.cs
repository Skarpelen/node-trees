using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace NodeTrees.Shared.DTO
{
    [SwaggerSchema(Description = "Generic range page with items and total count.")]
    public sealed class RangeDto<T>
    {
        [SwaggerSchema(Description = "Number of items skipped by server (offset).", Format = "int32")]
        [Required]
        public int Skip { get; set; }

        [SwaggerSchema(Description = "Maximum number of items in the response (page size).", Format = "int32")]
        [Required]
        public int Count { get; set; }

        [SwaggerSchema(Description = "Page items.")]
        [Required]
        public List<T> Items { get; set; } = new();
    }
}

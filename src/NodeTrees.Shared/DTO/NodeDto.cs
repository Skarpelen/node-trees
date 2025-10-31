using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace NodeTrees.Shared.DTO
{
    [SwaggerSchema(Description = "Node of the tree with nested children.")]
    public sealed class NodeDto
    {
        [SwaggerSchema(Description = "Node identifier.", Format = "int64")]
        [Required]
        public long Id { get; set; }

        [SwaggerSchema(Description = "Human-readable node name.")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Child nodes collection.")]
        [Required]
        public List<NodeDto> Children { get; set; } = new();
    }
}

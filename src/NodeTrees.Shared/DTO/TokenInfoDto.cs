using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace NodeTrees.Shared.DTO
{
    [SwaggerSchema(Description = "Bearer token info.")]
    public sealed class TokenInfoDto
    {
        [SwaggerSchema(Description = "Access token string.")]
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

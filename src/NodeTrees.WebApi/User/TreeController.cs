using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NodeTrees.WebApi.User
{
    using NodeTrees.BusinessLogic.Services;
    using NodeTrees.Shared.DTO;

    [Route("")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public sealed class TreeController(ITreeService service, IMapper mapper) : ControllerBase
    {
        [HttpPost]
        [Route("api.user.tree.get")]
        [ProducesResponseType(typeof(NodeDto), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Returns your entire tree. If your tree doesn't exist it will be created automatically.",
            Tags = new[] { "user.tree" }
        )]
        public async Task<ActionResult<NodeDto>> Get([FromQuery] string treeName)
        {
            var rootDomain = await service.GetTreeAsync(treeName);
            var rootDto = mapper.Map<NodeDto>(rootDomain);
            return Ok(rootDto);
        }
    }
}

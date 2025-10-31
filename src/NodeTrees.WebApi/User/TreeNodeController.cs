using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NodeTrees.WebApi.User
{
    using NodeTrees.BusinessLogic.Services;

    [Route("")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public sealed class TreeNodeController(ITreeService service) : ControllerBase
    {
        [HttpPost]
        [Route("api.user.tree.node.create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Create a new node in your tree. You must to specify a parent node ID that belongs to your tree or dont pass parent ID to create tree first level node. A new node name must be unique across all siblings.",
            Tags = new[] { "user.tree.node" }
        )]
        public async Task<IActionResult> Create(
            [FromQuery] string treeName,
            [FromQuery] long? parentNodeId,
            [FromQuery] string nodeName)
        {
            await service.CreateNodeAsync(treeName, parentNodeId, nodeName);
            return Ok();
        }

        [HttpPost]
        [Route("api.user.tree.node.delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Delete an existing node and all its descendants",
            Tags = new[] { "user.tree.node" }
        )]
        public async Task<IActionResult> Delete([FromQuery] long nodeId)
        {
            await service.DeleteNodeAsync(nodeId);
            return Ok();
        }

        [HttpPost]
        [Route("api.user.tree.node.rename")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Rename an existing node in your tree. A new name of the node must be unique across all siblings.",
            Tags = new[] { "user.tree.node" }
        )]
        public async Task<IActionResult> Rename(
            [FromQuery] long nodeId,
            [FromQuery] string newNodeName)
        {
            await service.RenameNodeAsync(nodeId, newNodeName);
            return Ok();
        }
    }
}

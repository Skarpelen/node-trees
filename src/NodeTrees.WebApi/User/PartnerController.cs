using Asp.Versioning;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public sealed class PartnerController(IUserService service) : ControllerBase
    {
        [HttpPost]
        [Route("api.user.partner.rememberMe")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenInfoDto), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Saves user by unique code and returns auth token required on all other requests, if implemented.",
            Tags = new[] { "user.partner" }
        )]
        public async Task<ActionResult<TokenInfoDto>> RememberMe([FromQuery] string code)
        {
            var token = await service.RememberMeAsync(code);
            var dto = new TokenInfoDto()
            {
                Token = token
            };

            return Ok(dto);
        }
    }
}

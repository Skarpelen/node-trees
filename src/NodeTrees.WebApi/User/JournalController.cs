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
    public sealed class JournalController(IJournalService service, IMapper mapper) : ControllerBase
    {
        [HttpPost]
        [Route("api.user.journal.getRange")]
        [ProducesResponseType(typeof(RangeDto<JournalInfoDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional.",
            Tags = new[] { "user.journal" }
        )]
        public async Task<ActionResult<RangeDto<JournalInfoDto>>> GetRange(
            [FromQuery] int skip,
            [FromQuery] int take,
            [FromBody] JournalFilterDto? filter)
        {
            var data = await service.GetRangeAsync(skip, take, filter);
            var dataDto = mapper.Map<RangeDto<JournalInfoDto>>(data);
            return Ok(dataDto);
        }

        [HttpPost]
        [Route("api.user.journal.getSingle")]
        [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "",
            Description = "Returns the information about an particular event by ID.",
            Tags = new[] { "user.journal" }
        )]
        public async Task<ActionResult<JournalDto>> GetSingle([FromQuery] long id)
        {
            var entity = await service.GetSingleAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<JournalDto>(entity);
            return Ok(dto);
        }
    }
}

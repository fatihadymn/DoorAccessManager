using DoorAccessManager.Api.Infrastructure.Authentication;
using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Items.Enums;
using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DoorAccessManager.Api.Controllers
{
    [Route("api/doors")]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    public class DoorController : BaseController
    {
        private readonly IDoorService _doorService;
        private readonly IContextAccessor _contextAccessor;

        public DoorController(IDoorService doorService, IContextAccessor contextAccessor)
        {
            _doorService = doorService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        [Authorize(Policy = nameof(RolePolicyTypes.All))]
        [ProducesResponseType(typeof(List<DoorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoors()
        {
            return Ok(await _doorService.GetOfficeDoorsByRoleAsync(new GetDoorsRequest()
            {
                OfficeId = _contextAccessor.OfficeId,
                RoleName = _contextAccessor.Role
            }));
        }

        [HttpPost("{id:guid}/access")]
        [Authorize(Policy = nameof(RolePolicyTypes.All))]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> AccessDoor([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id cannot be empty");
            }

            var result = await _doorService.AccessDoorAsync(new()
            {
                DoorId = id,
                UserId = _contextAccessor.UserId
            });

            return result ? Ok() : StatusCode((int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{id:guid}/access-logs")]
        [Authorize(Policy = nameof(RolePolicyTypes.Admin_OfficeManager))]
        [ProducesResponseType(typeof(List<DoorAccessLogResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoorAccessLogs([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id cannot be empty");
            }

            return Ok(await _doorService.GetDoorAccessLogsAsync(new GetDoorAccessLogsRequest()
            {
                DoorId = id,
                UserId = _contextAccessor.UserId
            }));
        }
    }
}

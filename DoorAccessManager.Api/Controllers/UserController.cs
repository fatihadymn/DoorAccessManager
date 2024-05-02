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
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IContextAccessor _contextAccessor;

        public UserController(IUserService userService, IContextAccessor contextAccessor)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _userService.LoginAsync(request);

            if (result == null || String.IsNullOrEmpty(result.Token))
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = nameof(RolePolicyTypes.Admin))]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            request.OfficeId = _contextAccessor.OfficeId;

            var result = await _userService.CreateUserAsync(request);

            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpGet]
        [Authorize(Policy = nameof(RolePolicyTypes.Admin))]
        [ProducesResponseType(typeof(List<GetUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var request = new GetUsersRequest()
            {
                OfficeId = _contextAccessor.OfficeId
            };

            return Ok(await _userService.GetUsersAsync(request));
        }


        [HttpPatch("{id:guid}/password")]
        [Authorize(Policy = nameof(RolePolicyTypes.All))]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserPassword([FromRoute] Guid id, [FromBody] UpdateUserPasswordRequest request)
        {
            if (id != _contextAccessor.UserId)
            {
                return BadRequest("Cannot update another user's password");
            }

            request.UserId = id;

            await _userService.UpdateUserPasswordAsync(request);

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = nameof(RolePolicyTypes.Admin))]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("User Id cannot be empty");
            }

            await _userService.DeleteUserAsync(new DeleteUserRequest()
            {
                OfficeId = _contextAccessor.OfficeId,
                UserId = id
            });

            return Ok();
        }
    }
}

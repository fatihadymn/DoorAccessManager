using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorAccessManager.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
    }
}

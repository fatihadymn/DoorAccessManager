using DoorAccessManager.Items.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DoorAccessManager.Api.Controllers
{
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public class BaseController : ControllerBase
    {
    }
}

using DoorAccessManager.Items.Enums;
using DoorAccessManager.Items.Exceptions;
using System.Security.Claims;

namespace DoorAccessManager.Api.Infrastructure.Authentication
{
    public class ContextAccessor : HttpContextAccessor, IContextAccessor
    {
        public string Token
        {
            get
            {
                if (HttpContext == null)
                {
                    return string.Empty;
                }

                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                    if (token == null)
                    {
                        return string.Empty;
                    }

                    if (token.StartsWith("Bearer"))
                    {
                        token = token.Replace("Bearer ", "");
                    }

                    return token;
                }

                return string.Empty;
            }
        }

        public RoleTypes Role
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new BusinessException("HttpContext is unavailable");
                }

                return HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Role)
                    ? (RoleTypes)Enum.Parse(typeof(RoleTypes), HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)!.Value)
                    : throw new BusinessException("Role could not be found for User");
            }
        }

        public Guid OfficeId
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new BusinessException("HttpContext is unavailable");
                }

                return HttpContext.User.HasClaim(x => x.Type == "officeId")
                    ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "officeId")!.Value)
                    : throw new BusinessException("Office could not be found for User"); ;
            }
        }

        public Guid UserId
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new BusinessException("HttpContext is unavailable");
                }

                return HttpContext.User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier)
                    ? Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value)
                    : throw new BusinessException("User could not be found");
            }
        }
    }
}

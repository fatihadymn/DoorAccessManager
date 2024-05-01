using DoorAccessManager.Items.Enums;

namespace DoorAccessManager.Api.Infrastructure.Authentication
{
    public interface IContextAccessor
    {
        string Token { get; }

        RoleTypes Role { get; }

        Guid OfficeId { get; }

        Guid UserId { get; }
    }
}

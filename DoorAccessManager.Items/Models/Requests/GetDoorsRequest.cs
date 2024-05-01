using DoorAccessManager.Items.Enums;

namespace DoorAccessManager.Items.Models.Requests
{
    public class GetDoorsRequest
    {
        public Guid OfficeId { get; set; }

        public RoleTypes RoleName { get; set; }
    }
}

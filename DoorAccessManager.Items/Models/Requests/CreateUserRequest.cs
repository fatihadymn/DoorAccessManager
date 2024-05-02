using DoorAccessManager.Items.Enums;
using System.Text.Json.Serialization;

namespace DoorAccessManager.Items.Models.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public RoleTypes Role { get; set; }

        [JsonIgnore]
        public Guid OfficeId { get; set; }
    }
}

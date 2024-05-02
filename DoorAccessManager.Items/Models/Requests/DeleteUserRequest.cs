using System.Text.Json.Serialization;

namespace DoorAccessManager.Items.Models.Requests
{
    public class DeleteUserRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Guid OfficeId { get; set; }
    }
}

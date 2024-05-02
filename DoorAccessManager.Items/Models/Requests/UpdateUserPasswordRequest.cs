using System.Text.Json.Serialization;

namespace DoorAccessManager.Items.Models.Requests
{
    public class UpdateUserPasswordRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}

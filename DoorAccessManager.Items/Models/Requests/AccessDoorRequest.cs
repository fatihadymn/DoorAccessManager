namespace DoorAccessManager.Items.Models.Requests
{
    public class AccessDoorRequest
    {
        public Guid UserId { get; set; }

        public Guid DoorId { get; set; }
    }
}

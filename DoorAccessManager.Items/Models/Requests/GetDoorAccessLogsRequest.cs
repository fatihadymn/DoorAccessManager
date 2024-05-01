namespace DoorAccessManager.Items.Models.Requests
{
    public class GetDoorAccessLogsRequest
    {
        public Guid UserId { get; set; }

        public Guid DoorId { get; set; }

    }
}

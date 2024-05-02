namespace DoorAccessManager.Items.Models.Responses
{
    public class DoorAccessLogResponse
    {
        public string FullName { get; set; }

        public string Username { get; set; }

        public string DoorName { get; set; }

        public bool IsSuccess { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

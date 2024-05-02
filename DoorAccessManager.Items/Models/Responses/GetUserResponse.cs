namespace DoorAccessManager.Items.Models.Responses
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }

        public string OfficeName { get; set; }

        public bool IsActive { get; set; }
    }
}

namespace DoorAccessManager.Items.Models.Responses
{
    public class CreateUserResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public bool IsActive { get; set; }
    }
}

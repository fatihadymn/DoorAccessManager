namespace DoorAccessManager.Items.Models.Responses
{
    public class DoorResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<string> DoorRoles { get; set; }
    }
}
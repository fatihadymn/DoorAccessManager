namespace DoorAccessManager.Items.Entities
{
    public class Office : BaseEntity
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Door> Doors { get; set; }

        public ICollection<User> Users { get; set; }
    }
}

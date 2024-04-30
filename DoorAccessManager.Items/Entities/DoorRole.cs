namespace DoorAccessManager.Items.Entities
{
    public class DoorRole : BaseEntity
    {
        public Guid DoorId { get; set; }

        public Door Door { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; }
    }
}

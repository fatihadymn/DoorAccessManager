namespace DoorAccessManager.Items.Entities
{
    public class Door : BaseEntity
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public Guid OfficeId { get; set; }

        public Office Office { get; set; }

        public ICollection<DoorRole> DoorRoles { get; set; }

        public ICollection<DoorAccessLog> DoorAccessLogs { get; set; }
    }
}

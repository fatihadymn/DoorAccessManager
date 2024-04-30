namespace DoorAccessManager.Items.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        public Guid OfficeId { get; set; }

        public Office Office { get; set; }

        public bool IsActive { get; set; }

        public ICollection<DoorAccessLog> DoorAccessLogs { get; set; }
    }
}

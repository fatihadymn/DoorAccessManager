using DoorAccessManager.Items.Enums;

namespace DoorAccessManager.Items.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<DoorRole> DoorRoles { get; set; }
    }
}

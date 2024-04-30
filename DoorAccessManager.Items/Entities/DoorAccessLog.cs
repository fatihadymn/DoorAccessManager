namespace DoorAccessManager.Items.Entities
{
    public class DoorAccessLog : BaseEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid DoorId { get; set; }

        public Door Door { get; set; }

        public bool IsSuccess { get; set; }

        public string Description { get; set; }
    }
}

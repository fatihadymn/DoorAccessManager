using DoorAccessManager.Items.Entities;

namespace DoorAccessManager.Data.Repositories.Abstract
{
    public interface IDoorRepository : IRepositoryBase
    {
        Task<List<Door>> GetOfficeDoorsByRoleAsync(Guid officeId, string roleName);

        Task<bool> CheckAccessDoor(Guid userId, Guid doorId);

        Task<bool> IsDoorExist(Guid doorId);
    }
}

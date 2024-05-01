using DoorAccessManager.Items.Entities;

namespace DoorAccessManager.Data.Repositories.Abstract
{
    public interface IDoorAccessLogRepository : IRepositoryBase
    {
        Task CreateAccessLogAsync(DoorAccessLog doorAccessLog);

        Task<List<DoorAccessLog>> GetDoorAccessLogsByDoorIdAsync(Guid doorId, Guid userId);
    }
}

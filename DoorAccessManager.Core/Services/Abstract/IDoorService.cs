using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;

namespace DoorAccessManager.Core.Services.Abstract
{
    public interface IDoorService : IServiceBase
    {
        Task<List<DoorResponse>> GetOfficeDoorsByRoleAsync(GetDoorsRequest request);

        Task<bool> AccessDoorAsync(AccessDoorRequest request);

        Task<List<DoorAccessLogResponse>> GetDoorAccessLogsAsync(GetDoorAccessLogsRequest request);
    }
}

using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Exceptions;
using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;
using MapsterMapper;

namespace DoorAccessManager.Core.Services
{
    public class DoorService : ServiceBase, IDoorService
    {
        private readonly IDoorRepository _doorRepository;
        private readonly IDoorAccessLogRepository _doorAccessLogRepository;
        private readonly IMapper _mapper;

        public DoorService(IDoorRepository doorRepository, IMapper mapper, IDoorAccessLogRepository doorAccessLogRepository)
        {
            _doorRepository = doorRepository;
            _mapper = mapper;
            _doorAccessLogRepository = doorAccessLogRepository;
        }

        public async Task<List<DoorResponse>> GetOfficeDoorsByRoleAsync(GetDoorsRequest request)
        {
            if (request == null || request.OfficeId == Guid.Empty)
                throw new BusinessException("Get Doors request is missing");

            var doors = await _doorRepository.GetOfficeDoorsByRoleAsync(request.OfficeId, request.RoleName.ToString());

            return _mapper.Map<List<DoorResponse>>(doors);
        }

        public async Task<List<DoorAccessLogResponse>> GetDoorAccessLogsAsync(GetDoorAccessLogsRequest request)
        {
            if (request == null || request.UserId == Guid.Empty || request.DoorId == Guid.Empty)
            {
                throw new BusinessException("Get Door Access Logs request is missing");
            }

            var accessLogs = await _doorAccessLogRepository.GetDoorAccessLogsByDoorIdAsync(request.DoorId, request.UserId);

            return _mapper.Map<List<DoorAccessLogResponse>>(accessLogs);
        }

        public async Task<bool> AccessDoorAsync(AccessDoorRequest request)
        {
            if (request == null || request.DoorId == Guid.Empty || request.UserId == Guid.Empty)
            {
                throw new BusinessException("Door and User informations cannot be empty");
            }

            if (!await _doorRepository.IsDoorExist(request.DoorId))
            {
                throw new BusinessException("Door is not exist");
            }

            var result = true;
            var description = "User accessed the door successfully";

            try
            {
                result = await _doorRepository.CheckAccessDoor(request.UserId, request.DoorId);

                if (!result)
                {
                    description = "User cannot access this door";
                }

                return result;
            }
            catch (Exception ex)
            {
                description = ex.Message;

                result = false;

                throw;
            }
            finally
            {
                await _doorAccessLogRepository.CreateAccessLogAsync(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Description = description,
                    CreatedOn = DateTime.UtcNow,
                    DoorId = request.DoorId,
                    IsSuccess = result
                });
            }
        }
    }
}

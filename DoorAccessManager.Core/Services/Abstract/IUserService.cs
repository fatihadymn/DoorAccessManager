using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;

namespace DoorAccessManager.Core.Services.Abstract
{
    public interface IUserService : IServiceBase
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}

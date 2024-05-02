using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Enums;

namespace DoorAccessManager.Data.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string userName, string password);

        Task<User> CreateUserAsync(User user, RoleTypes role);

        Task<List<User>> GetUsersByOfficeIdAsync(Guid officeId);

        Task UpdateUserPasswordAsync(Guid userId, string oldPassword, string newPassword);

        Task DeleteUserAsync(Guid userId, Guid officeId);
    }
}

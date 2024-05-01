using DoorAccessManager.Items.Entities;

namespace DoorAccessManager.Data.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string userName, string password);
    }
}

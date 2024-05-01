using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Exceptions;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace DoorAccessManager.Data.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new BusinessException("Username or Password cannot be null or empty");
            }

            var existUser = await _context.Users.Where(x => x.Username == userName && x.IsActive)
                                                .Include(x => x.Role)
                                                .FirstOrDefaultAsync();

            if (existUser == null)
            {
                throw new BusinessException($"Could not find user {userName}");
            }

            if (!BC.Verify(password, existUser.PasswordHash))
            {
                throw new BusinessException("Username or password might be wrong");
            }

            return existUser;
        }
    }
}

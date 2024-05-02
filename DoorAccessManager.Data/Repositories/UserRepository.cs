using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Enums;
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

        public async Task<User> CreateUserAsync(User user, RoleTypes role)
        {
            if (user == null)
            {
                throw new BusinessException("User cannot be null");
            }

            if (await _context.Users.AnyAsync(x => x.Username == user.Username))
            {
                throw new BusinessException("Username is using by another one");
            }

            user.RoleId = await _context.Roles.Where(x => x.Name == role.ToString()).Select(x => x.Id).FirstOrDefaultAsync();

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetUsersByOfficeIdAsync(Guid officeId)
        {
            if (officeId == Guid.Empty)
            {
                throw new BusinessException("Office cannot be null");
            }

            return await _context.Users.Where(x => x.OfficeId == officeId)
                                       .Include(x => x.Role)
                                       .Include(x => x.Office)
                                       .ToListAsync();
        }

        public async Task UpdateUserPasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(x => x.IsActive && x.Id == userId);

            if (existUser == null)
            {
                throw new BusinessException("User cannot be found");
            }

            if (!BC.Verify(oldPassword, existUser.PasswordHash))
            {
                throw new BusinessException("Old Password is wrong");
            }

            existUser.PasswordHash = BC.HashPassword(newPassword);
            existUser.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userId, Guid officeId)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.OfficeId == officeId);

            if (existUser == null)
            {
                throw new BusinessException("User cannot be found");
            }

            existUser.IsActive = false;
            existUser.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}

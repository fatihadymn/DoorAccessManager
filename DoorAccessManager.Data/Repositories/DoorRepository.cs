using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DoorAccessManager.Data.Repositories
{
    public class DoorRepository : RepositoryBase, IDoorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Door>> GetOfficeDoorsByRoleAsync(Guid officeId, string roleName)
        {
            if (officeId == Guid.Empty || String.IsNullOrEmpty(roleName))
            {
                throw new BusinessException("Office Id and Role Name cannot be null or empty");
            }

            return await _context.Doors.Where(x => x.IsActive &&
                                                   x.OfficeId == officeId &&
                                                   x.DoorRoles.Select(y => y.Role.Name).Contains(roleName))
                                       .Include(x => x.DoorRoles)
                                       .ThenInclude(x => x.Role)
                                       .ToListAsync();
        }

        public async Task<bool> CheckAccessDoor(Guid userId, Guid doorId)
        {
            if (userId == Guid.Empty || doorId == Guid.Empty)
            {
                throw new BusinessException("User, Door or Office informations cannot be empty");
            }

            var result = await _context.Users.AnyAsync(x => x.Id == userId &&
                                                           x.IsActive &&
                                                           x.Office.IsActive &&
                                                           x.Office.Doors.Any(y => y.Id == doorId &&
                                                                                   y.IsActive &&
                                                                                   y.DoorRoles.Select(z => z.RoleId).Contains(x.RoleId)));

            return result;
        }

        public async Task<bool> IsDoorExist(Guid doorId)
        {
            if (doorId == Guid.Empty)
            {
                throw new BusinessException("Door Id cannot be empty");
            }

            return await _context.Doors.AnyAsync(x => x.Id == doorId && x.IsActive);
        }
    }
}

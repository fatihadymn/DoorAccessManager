using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DoorAccessManager.Data.Repositories
{
    public class DoorAccessLogRepository : RepositoryBase, IDoorAccessLogRepository
    {
        private readonly ApplicationDbContext _context;

        public DoorAccessLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccessLogAsync(DoorAccessLog doorAccessLog)
        {
            if (doorAccessLog == null)
            {
                throw new BusinessException("Door Access Log cannot be empty");
            }

            await _context.DoorAccessLogs.AddAsync(doorAccessLog);

            await _context.SaveChangesAsync();
        }

        public async Task<List<DoorAccessLog>> GetDoorAccessLogsByDoorIdAsync(Guid doorId, Guid userId)
        {
            if (doorId == Guid.Empty || userId == Guid.Empty)
            {
                throw new BusinessException("Door or User informations cannot be empty");
            }

            var hasUserAccess = await _context.Users.AnyAsync(x => x.Id == userId &&
                                                                   x.IsActive &&
                                                                   x.Office.IsActive &&
                                                                   x.Office.Doors.Any(y => y.Id == doorId &&
                                                                                           y.IsActive &&
                                                                                           y.DoorRoles.Select(z => z.RoleId).Contains(x.RoleId)));

            if (!hasUserAccess)
            {
                throw new BusinessException("This user cannot see the door's access logs");
            }

            return await _context.DoorAccessLogs.Where(x => x.DoorId == doorId)
                                                .Include(x => x.User)
                                                .Include(x => x.Door)
                                                .ToListAsync();
        }
    }
}

using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoorAccessManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataIdentifier).Assembly);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Office> Offices { get; set; }

        public DbSet<Door> Doors { get; set; }

        public DbSet<DoorRole> DoorRoles { get; set; }

        public DbSet<DoorAccessLog> DoorAccessLogs { get; set; }
    }
}

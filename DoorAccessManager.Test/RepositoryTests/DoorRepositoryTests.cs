namespace DoorAccessManager.Test.RepositoryTests
{
    public class DoorRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
        private DoorRepository _repository;

        public DoorRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DoorRepositoryTests")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new ApplicationDbContext(_contextOptions);

            Initializer.PrepareUnitTestData(context);
        }

        [Fact]
        public async Task GetOfficeDoorsByRoleAsync_WithValidOfficeIdAndRole_ReturnsMatchingDoors()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorRepository(context);

            var officeId = await context.Offices.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync();

            // Act
            var result = await _repository.GetOfficeDoorsByRoleAsync(officeId, RoleTypes.Admin.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(officeId, result[0].OfficeId);
        }

        [Fact]
        public async Task CheckAccessDoor_WithValidUserAndDoor_ReturnsFalse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorRepository(context);

            var userId = Guid.NewGuid();
            var doorId = Guid.NewGuid();

            // Act
            var result = await _repository.CheckAccessDoor(userId, doorId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckAccessDoor_WithValidUserAndDoor_ReturnsTrue()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorRepository(context);

            var userId = await context.Users.Where(x => x.Role.Name == RoleTypes.Admin.ToString()).Select(x => x.Id).FirstOrDefaultAsync();
            var doorId = await context.DoorRoles.Where(x => x.Role.Name == RoleTypes.Admin.ToString()).Select(x => x.DoorId).FirstOrDefaultAsync();

            // Act
            var result = await _repository.CheckAccessDoor(userId, doorId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsDoorExist_WithValidDoorId_ReturnsTrue()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorRepository(context);

            var doorId = await context.Doors.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync();

            // Act
            var result = await _repository.IsDoorExist(doorId);

            // Assert
            Assert.True(result);
        }

    }
}

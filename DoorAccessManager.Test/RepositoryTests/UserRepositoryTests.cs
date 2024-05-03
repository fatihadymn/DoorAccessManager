namespace DoorAccessManager.Test.RepositoryTests
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
        private UserRepository _repository;

        public UserRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("UserRepositoryTests")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new ApplicationDbContext(_contextOptions);

            Initializer.PrepareUnitTestData(context);
        }

        [Fact]
        public async Task GetUserAsync_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var userName = "first_emp";
            var password = "123";

            // Act
            var result = await _repository.GetUserAsync(userName, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userName, result.Username);
        }

        [Fact]
        public async Task GetUserAsync_WithInvalidCredentials_ThrowsBusinessException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var userName = "nonexistentuser";
            var password = "wrongpassword";

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _repository.GetUserAsync(userName, password);
            });
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUser_CreateUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var officeId = await context.Offices.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync();
            var roleId = await context.Roles.Where(x => x.Name == RoleTypes.Employee.ToString()).Select(x => x.Id).FirstOrDefaultAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "newuser",
                PasswordHash = BC.HashPassword("newpassword"),
                IsActive = true,
                OfficeId = officeId,
                CreatedOn = DateTime.UtcNow,
                Name = "Test",
                RoleId = roleId
            };

            // Act
            var result = await _repository.CreateUserAsync(user, RoleTypes.Employee);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.PasswordHash, result.PasswordHash);
        }

        [Fact]
        public async Task GetUsersByOfficeIdAsync_WithValidOfficeId_ReturnsUsers()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var officeId = await context.Offices.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync();

            // Act
            var result = await _repository.GetUsersByOfficeIdAsync(officeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(officeId, result[0].OfficeId);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task UpdateUserPasswordAsync_WithValidUserAndPassword_UpdatesPassword()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var userId = await context.Users.Where(x => x.IsActive).Select(x => x.Id).FirstOrDefaultAsync();
            var oldPassword = "123";
            var newPassword = "1234";

            // Act
            await _repository.UpdateUserPasswordAsync(userId, oldPassword, newPassword);

            var updatedUser = await context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            // Assert
            Assert.True(BC.Verify(newPassword, updatedUser!.PasswordHash));
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidUserAndOffice_DeactivatesUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new UserRepository(context);

            var existUser = await context.Users.Where(x => x.IsActive).FirstOrDefaultAsync();

            var userId = existUser!.Id;
            var officeId = existUser!.OfficeId;

            // Act
            await _repository.DeleteUserAsync(userId, officeId);

            var updatedUser = await context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            // Assert
            Assert.False(updatedUser!.IsActive);
        }

    }
}

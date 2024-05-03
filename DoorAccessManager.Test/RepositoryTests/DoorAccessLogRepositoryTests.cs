namespace DoorAccessManager.Test.RepositoryTests
{
    public class DoorAccessLogRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
        private DoorAccessLogRepository _repository;

        public DoorAccessLogRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DoorAccessLogRepositoryTests")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new ApplicationDbContext(_contextOptions);

            Initializer.PrepareUnitTestData(context);
        }

        [Fact]
        public async Task CreateAccessLogAsync_WithValidAccessLog_SavesToDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorAccessLogRepository(context);

            var accessLog = new DoorAccessLog
            {
                Id = Guid.NewGuid(),
                DoorId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Description = "description",
                IsSuccess = true
            };

            // Act
            await _repository.CreateAccessLogAsync(accessLog);

            var result = await context.DoorAccessLogs.FirstOrDefaultAsync(x => x.Id == accessLog.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accessLog.Description, result.Description);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task CreateAccessLogAsync_WithEmptyAccessLog_ThrowsBusinessException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorAccessLogRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _repository.CreateAccessLogAsync(null);
            });
        }

        [Fact]
        public async Task GetDoorAccessLogsByDoorIdAsync_WithUnauthorizedUser_ThrowsBusinessException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_contextOptions);
            _repository = new DoorAccessLogRepository(context);

            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _repository.GetDoorAccessLogsByDoorIdAsync(doorId, userId);
            });
        }

    }
}

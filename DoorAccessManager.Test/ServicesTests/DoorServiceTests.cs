namespace DoorAccessManager.Test.ServicesTests
{
    public class DoorServiceTests
    {
        private readonly Mock<IDoorRepository> _mockDoorRepository;
        private readonly Mock<IDoorAccessLogRepository> _mockDoorAccessLogRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DoorService _doorService;

        public DoorServiceTests()
        {
            // Setup mock dependencies
            _mockDoorRepository = new Mock<IDoorRepository>();
            _mockDoorAccessLogRepository = new Mock<IDoorAccessLogRepository>();
            _mockMapper = new Mock<IMapper>();

            _doorService = new DoorService(_mockDoorRepository.Object, _mockMapper.Object, _mockDoorAccessLogRepository.Object);
        }

        [Fact]
        public async Task GetOfficeDoorsByRoleAsync_WithValidRequest_ReturnsMappedDoorList()
        {
            // Arrange

            var officeId = Guid.NewGuid();
            var roleName = RoleTypes.Admin;
            var doorsFromRepo = new List<Door>
            {
                new Door { Id = Guid.NewGuid(), Name = "Door 1", CreatedOn = DateTime.UtcNow, IsActive = true },
                new Door { Id = Guid.NewGuid(), Name = "Door 2", CreatedOn = DateTime.UtcNow, IsActive = true }
            };

            var doorResponse = new List<DoorResponse>
            {
                new DoorResponse
                {
                    Id = doorsFromRepo.First().Id,
                    DoorRoles = new List<string>{RoleTypes.Admin.ToString(), RoleTypes.Employee.ToString() },
                    Name = "Door 1"
                },
                new DoorResponse
                {
                    Id = doorsFromRepo.Last().Id,
                    DoorRoles = new List<string>{RoleTypes.Admin.ToString(), RoleTypes.Employee.ToString() },
                    Name = "Door 2"
                }
            };

            _mockMapper.Setup(x => x.Map<List<DoorResponse>>(It.IsAny<List<Door>>()))
                       .Returns(doorResponse);

            _mockDoorRepository.Setup(repo => repo.GetOfficeDoorsByRoleAsync(officeId, roleName.ToString()))
                              .ReturnsAsync(doorsFromRepo);

            var request = new GetDoorsRequest { OfficeId = officeId, RoleName = roleName };

            // Act
            var result = await _doorService.GetOfficeDoorsByRoleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(doorsFromRepo.Count, result.Count);
        }

        [Fact]
        public async Task GetDoorAccessLogsAsync_WithValidRequest_ReturnsMappedAccessLogsList()
        {
            // Arrange
            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var accessLogsFromRepo = new List<DoorAccessLog>
            {
                new DoorAccessLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DoorId = doorId,
                    Description = "Access log 1",
                    User = new User
                    {
                        Name = "test",
                        Username = "test",
                    },
                    Door = new Door
                    {
                        Name = "test"
                    }
                },
                new DoorAccessLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DoorId = doorId,
                    Description = "Access log 2",
                    User = new User
                    {
                        Name = "test",
                        Username = "test",
                    },
                    Door = new Door
                    {
                        Name = "test"
                    }
                }
            };

            var accessLogsResponse = new List<DoorAccessLogResponse>
            {
                new DoorAccessLogResponse
                {
                    FullName = "test",
                    DoorName = "test",
                    IsSuccess = true,
                    Username= "test",
                    CreatedOn = DateTime.UtcNow,
                    Description = "Access log 1"
                },
                new DoorAccessLogResponse
                {
                    FullName = "test",
                    DoorName = "test",
                    IsSuccess = true,
                    Username= "test",
                    CreatedOn = DateTime.UtcNow,
                    Description = "Access log 2"
                },
            };

            _mockDoorAccessLogRepository.Setup(x => x.GetDoorAccessLogsByDoorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                                        .ReturnsAsync(accessLogsFromRepo);

            _mockMapper.Setup(x => x.Map<List<DoorAccessLogResponse>>(It.IsAny<List<DoorAccessLog>>()))
                       .Returns(accessLogsResponse);

            var request = new GetDoorAccessLogsRequest { DoorId = doorId, UserId = userId };

            // Act
            var result = await _doorService.GetDoorAccessLogsAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accessLogsFromRepo.Count, result.Count);
        }

        [Fact]
        public async Task AccessDoorAsync_WithValidRequest_ReturnsTrueAndCreatesAccessLog()
        {
            // Arrange
            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new AccessDoorRequest { DoorId = doorId, UserId = userId };

            _mockDoorRepository.Setup(repo => repo.IsDoorExist(doorId))
                               .ReturnsAsync(true);
            _mockDoorRepository.Setup(repo => repo.CheckAccessDoor(userId, doorId))
                               .ReturnsAsync(true);

            // Act
            var result = await _doorService.AccessDoorAsync(request);

            // Assert
            Assert.True(result);
            _mockDoorAccessLogRepository.Verify(repo => repo.CreateAccessLogAsync(It.IsAny<DoorAccessLog>()), Times.Once);
        }

        [Fact]
        public async Task AccessDoorAsync_WithInvalidRequest_ThrowsBusinessException()
        {
            // Arrange
            var request = new AccessDoorRequest { DoorId = Guid.Empty, UserId = Guid.Empty };

            // Create DoorService instance
            var doorService = new DoorService(null, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => doorService.AccessDoorAsync(request));
        }

        [Fact]
        public async Task AccessDoorAsync_WhenDoorNotExist_ThrowsBusinessException()
        {
            // Arrange
            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new AccessDoorRequest { DoorId = doorId, UserId = userId };

            _mockDoorRepository.Setup(repo => repo.IsDoorExist(doorId))
                              .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _doorService.AccessDoorAsync(request));
        }
    }
}

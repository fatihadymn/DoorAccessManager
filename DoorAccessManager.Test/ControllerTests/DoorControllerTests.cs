namespace DoorAccessManager.Test.ControllerTests
{
    public class DoorControllerTests
    {
        private readonly Mock<IDoorService> _mockDoorService;
        private readonly Mock<IContextAccessor> _mockContextAccessor;
        private readonly DoorController _doorController;

        public DoorControllerTests()
        {
            // Setup mock dependencies
            _mockDoorService = new Mock<IDoorService>();
            _mockContextAccessor = new Mock<IContextAccessor>();
            _doorController = CreateInstance(_mockDoorService, _mockContextAccessor);
        }

        [Fact]
        public async Task GetOfficeDoorsByRole_WithValidOfficeIdAndRole_ReturnsListOfDoorResponses()
        {
            // Arrange
            var officeId = Guid.NewGuid();
            var roleName = RoleTypes.Admin;
            var expectedDoors = new List<DoorResponse>
            {
                new DoorResponse { Id = Guid.NewGuid(), Name = "Door 1" },
                new DoorResponse { Id = Guid.NewGuid(), Name = "Door 2" }
            };

            _mockDoorService.Setup(x => x.GetOfficeDoorsByRoleAsync(It.IsAny<GetDoorsRequest>()))
                            .ReturnsAsync(expectedDoors);

            _mockContextAccessor.SetupGet(x => x.OfficeId).Returns(officeId);
            _mockContextAccessor.SetupGet(x => x.Role).Returns(roleName);

            // Act
            var result = await _doorController.GetDoors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDoors = Assert.IsAssignableFrom<List<DoorResponse>>(okResult.Value);
            Assert.Equal(expectedDoors.Count, actualDoors.Count);

            _mockDoorService.Verify(x => x.GetOfficeDoorsByRoleAsync(It.IsAny<GetDoorsRequest>()), Times.Once);
        }

        [Fact]
        public async Task GetOfficeDoorsByRole_WithMissingOfficeId_ThrownException()
        {
            // Arrange
            try
            {
                _mockDoorService.Setup(x => x.GetOfficeDoorsByRoleAsync(It.IsAny<GetDoorsRequest>()))
                                .ReturnsAsync(() => throw new BusinessException("OfficeId cannot be empty"));

                _mockContextAccessor.SetupGet(x => x.OfficeId).Returns(Guid.Empty);

                // Act
                var result = await _doorController.GetDoors();

            }
            catch (BusinessException ex)
            {
                // Assert
                Assert.IsType<BusinessException>(ex);
            }
        }

        [Fact]
        public async Task AccessDoor_WithValidDoorIdAndUser_ReturnsOkResult()
        {
            // Arrange
            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockContextAccessor.SetupGet(x => x.UserId).Returns(userId);
            _mockDoorService.Setup(x => x.AccessDoorAsync(It.IsAny<AccessDoorRequest>()))
                            .ReturnsAsync(true);

            // Act
            var result = await _doorController.AccessDoor(doorId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            _mockDoorService.Verify(x => x.AccessDoorAsync(It.Is<AccessDoorRequest>(req => req.DoorId == doorId && req.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AccessDoor_WithEmptyDoorId_ThrownException()
        {
            // Arrange
            _mockDoorService.Setup(x => x.AccessDoorAsync(It.IsAny<AccessDoorRequest>()))
                           .ReturnsAsync(() => throw new BusinessException("Door cannot be found"));

            try
            {
                // Act
                var result = await _doorController.AccessDoor(Guid.Empty);
            }
            catch (BusinessException ex)
            {
                // Assert
                Assert.IsType<BusinessException>(ex);
            }
        }

        [Fact]
        public async Task GetDoorAccessLogs_WithValidDoorId_ReturnsListOfAccessLogs()
        {
            // Arrange
            var doorId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockContextAccessor.SetupGet(x => x.UserId).Returns(userId);

            var accessLogList = new List<DoorAccessLogResponse>
            {
                new DoorAccessLogResponse { CreatedOn = DateTime.UtcNow, Username = "test", DoorName = "test", FullName = "test", IsSuccess = false },
                new DoorAccessLogResponse { CreatedOn = DateTime.UtcNow, Username = "test", DoorName = "test", FullName = "test", IsSuccess = true }
            };

            _mockDoorService.Setup(x => x.GetDoorAccessLogsAsync(It.IsAny<GetDoorAccessLogsRequest>()))
                            .ReturnsAsync(accessLogList);

            // Act
            var result = await _doorController.GetDoorAccessLogs(doorId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var accessLogs = Assert.IsType<List<DoorAccessLogResponse>>(okResult.Value);
            Assert.Equal(2, accessLogs.Count);
        }

        [Fact]
        public async Task GetDoorAccessLogs_WithEmptyDoorId_ReturnsBadRequest()
        {
            // Arrange
            _mockDoorService.Setup(x => x.GetDoorAccessLogsAsync(It.IsAny<GetDoorAccessLogsRequest>()))
                            .ReturnsAsync(() => throw new BusinessException("Door cannot be found"));

            try
            {
                // Act
                var result = await _doorController.GetDoorAccessLogs(Guid.Empty);
            }
            catch (BusinessException ex)
            {
                // Assert
                Assert.IsType<BusinessException>(ex);
            }
        }

        private DoorController CreateInstance(Mock<IDoorService> doorServiceMock, Mock<IContextAccessor> mockContextAccessor)
        {
            var doorController = new DoorController(doorServiceMock.Object, mockContextAccessor.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            doorController.ControllerContext = new ControllerContext();

            return doorController;
        }
    }
}

namespace DoorAccessManager.Test.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IContextAccessor> _mockContextAccessor;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            // Setup mock dependencies
            _mockUserService = new Mock<IUserService>();
            _mockContextAccessor = new Mock<IContextAccessor>();
            _userController = CreateInstance(_mockUserService, _mockContextAccessor);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultWithToken()
        {
            // Arrange
            var validLoginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password"
            };

            var validLoginResponse = new LoginResponse
            {
                Token = "valid_token"
            };

            _mockUserService.Setup(x => x.LoginAsync(validLoginRequest))
                            .ReturnsAsync(validLoginResponse);

            // Act
            var result = await _userController.Login(validLoginRequest);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var response = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("valid_token", response.Token);
        }

        [Fact]
        public async Task CreateUser_WithValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var officeId = Guid.NewGuid();

            var validCreateUserRequest = new CreateUserRequest
            {
                Username = "newuser",
                Password = "newpassword",
                Name = "name",
                OfficeId = officeId,
                Role = RoleTypes.Employee
            };

            var validCreateUserResponse = new CreateUserResponse
            {
                Id = Guid.NewGuid(),
                Username = validCreateUserRequest.Username,
                Name = "name",
                IsActive = true
            };

            _mockContextAccessor.SetupGet(x => x.OfficeId).Returns(officeId);

            _mockUserService.Setup(x => x.CreateUserAsync(validCreateUserRequest))
                            .ReturnsAsync(validCreateUserResponse);

            // Act
            var result = await _userController.CreateUser(validCreateUserRequest);

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task GetUsers_WithValidOfficeId_ReturnsListOfUsers()
        {
            // Arrange
            var officeId = Guid.NewGuid();

            var userList = new List<GetUserResponse>
            {
                new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    IsActive = true,
                    Name = "test1",
                    OfficeName = "test1",
                    Role = RoleTypes.Employee.ToString()
                },
                new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    Username = "user2",
                    IsActive = true,
                    Name = "test2",
                    OfficeName = "test2",
                    Role = RoleTypes.Employee.ToString()
                }
            };

            _mockContextAccessor.SetupGet(x => x.OfficeId).Returns(officeId);
            _mockUserService.Setup(x => x.GetUsersAsync(It.IsAny<GetUsersRequest>()))
                            .ReturnsAsync(userList);

            // Act
            var result = await _userController.GetUsers();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var users = Assert.IsType<List<GetUserResponse>>(okResult.Value);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task UpdateUserPassword_WithValidRequestAndMatchingUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            _mockContextAccessor.SetupGet(x => x.UserId).Returns(userId);

            var updatePasswordRequest = new UpdateUserPasswordRequest
            {
                NewPassword = "newpassword",
                OldPassword = "oldpassword",
                UserId = userId,
            };

            // Act
            var result = await _userController.UpdateUserPassword(userId, updatePasswordRequest);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_WithValidUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            _mockContextAccessor.SetupGet(x => x.OfficeId).Returns(Guid.NewGuid());
            _mockContextAccessor.SetupGet(x => x.UserId).Returns(userId);

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            _mockUserService.Verify(x => x.DeleteUserAsync(It.Is<DeleteUserRequest>(req => req.UserId == userId)), Times.Once);
        }

        private UserController CreateInstance(Mock<IUserService> userServiceMock, Mock<IContextAccessor> mockContextAccessor)
        {
            var userController = new UserController(userServiceMock.Object, mockContextAccessor.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            userController.ControllerContext = new ControllerContext();

            return userController;
        }
    }
}

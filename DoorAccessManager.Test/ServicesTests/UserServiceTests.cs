namespace DoorAccessManager.Test.ServicesTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IConfiguration _configuration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Setup mock dependencies
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();

            var appSettings = JsonConvert.SerializeObject(new
            {
                Jwt = new JwtOptions
                {
                    SecretKey = "DTYSk5fXM9tGAcGL-oFM_DlQ5y1d5yoPg9PuAQhSgZs",
                    ValidIssuer = "localtest",
                    ExpirySeconds = 3600
                }
            });

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            _configuration = builder.Build();

            _userService = new UserService(_mockUserRepository.Object, _configuration, _mockMapper.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var validLoginRequest = new LoginRequest { Username = "testuser", Password = "password123" };
            var userFromRepository = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = new Role { Name = "Admin" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserAsync(validLoginRequest.Username, validLoginRequest.Password))
                               .ReturnsAsync(userFromRepository);

            // Act
            var result = await _userService.LoginAsync(validLoginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsBusinessException()
        {
            // Arrange
            var invalidLoginRequest = new LoginRequest { Username = "invaliduser", Password = "invalidpassword" };

            _mockUserRepository.Setup(repo => repo.GetUserAsync(invalidLoginRequest.Username, invalidLoginRequest.Password))
                               .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.LoginAsync(invalidLoginRequest));
        }

        [Fact]
        public async Task CreateUserAsync_WithValidRequest_ReturnsCreateUserResponse()
        {
            // Arrange
            var validCreateUserRequest = new CreateUserRequest
            {
                Username = "newuser",
                Password = "newpassword",
                Name = "New User",
                OfficeId = Guid.NewGuid(),
                Role = RoleTypes.Employee
            };

            var createdUser = new User
            {
                Id = Guid.NewGuid(),
                Username = validCreateUserRequest.Username,
                Name = validCreateUserRequest.Name,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
            };

            _mockUserRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<User>(), validCreateUserRequest.Role))
                               .ReturnsAsync(createdUser);

            var response = new CreateUserResponse()
            {
                Id = createdUser.Id,
                IsActive = createdUser.IsActive,
                Name = createdUser.Name,
                Username = createdUser.Username
            };

            _mockMapper.Setup(x => x.Map<CreateUserResponse>(It.IsAny<User>()))
                      .Returns(response);

            // Act
            var result = await _userService.CreateUserAsync(validCreateUserRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(validCreateUserRequest.Username, result.Username);
            Assert.Equal(validCreateUserRequest.Name, result.Name);
        }

        [Fact]
        public async Task GetUsersAsync_WithValidRequest_ReturnsListOfUsers()
        {
            // Arrange
            var validGetUsersRequest = new GetUsersRequest { OfficeId = Guid.NewGuid() };
            var usersFromRepository = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    Name = "User 1",
                    OfficeId = validGetUsersRequest.OfficeId,
                    IsActive = true,
                    Office = new Office { Name = "test" },
                    Role = new Role { Name = RoleTypes.Employee.ToString() }
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user2",
                    Name = "User 2",
                    OfficeId = validGetUsersRequest.OfficeId,
                    IsActive = true,
                    Office = new Office { Name = "test" },
                    Role = new Role { Name = RoleTypes.Employee.ToString() }
                },
            };

            var response = new List<GetUserResponse>()
            {
                new GetUserResponse()
                {
                    Id = usersFromRepository.First().Id,
                    Username = usersFromRepository.First().Name,
                    IsActive = usersFromRepository.First().IsActive,
                    Name = usersFromRepository.First().Name,
                    OfficeName = usersFromRepository.First().Office.Name,
                    Role =usersFromRepository.First().Role.Name
                },
                new GetUserResponse()
                {
                    Id = usersFromRepository.Last().Id,
                    Username = usersFromRepository.Last().Name,
                    IsActive = usersFromRepository.Last().IsActive,
                    Name = usersFromRepository.Last().Name,
                    OfficeName = usersFromRepository.Last().Office.Name,
                    Role =usersFromRepository.Last().Role.Name
                }
            };

            _mockUserRepository.Setup(repo => repo.GetUsersByOfficeIdAsync(validGetUsersRequest.OfficeId))
                               .ReturnsAsync(usersFromRepository);

            _mockMapper.Setup(x => x.Map<List<GetUserResponse>>(It.IsAny<List<User>>()))
                      .Returns(response);

            // Act
            var result = await _userService.GetUsersAsync(validGetUsersRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usersFromRepository.Count, result.Count);
        }

        [Fact]
        public async Task UpdateUserPasswordAsync_WithValidRequest_UpdatesPassword()
        {
            // Arrange
            var validUpdateRequest = new UpdateUserPasswordRequest
            {
                UserId = Guid.NewGuid(),
                OldPassword = "oldpassword",
                NewPassword = "newpassword"
            };

            // Mock GetUserAsync to return a user with matching UserId and hashed OldPassword
            _mockUserRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                               .ReturnsAsync(new User
                               {
                                   Id = validUpdateRequest.UserId,
                                   PasswordHash = BCrypt.Net.BCrypt.HashPassword(validUpdateRequest.OldPassword)
                               });

            // Act
            await _userService.UpdateUserPasswordAsync(validUpdateRequest);

            // Assert: Verify that UpdateUserPasswordAsync method was called on repository with correct parameters
            _mockUserRepository.Verify(repo => repo.UpdateUserPasswordAsync(validUpdateRequest.UserId, validUpdateRequest.OldPassword, validUpdateRequest.NewPassword), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidRequest_DeletesUser()
        {
            // Arrange
            var validDeleteRequest = new DeleteUserRequest
            {
                UserId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid()
            };

            // Act
            await _userService.DeleteUserAsync(validDeleteRequest);

            // Assert: Verify that DeleteUserAsync method was called on repository with correct parameters
            _mockUserRepository.Verify(repo => repo.DeleteUserAsync(validDeleteRequest.UserId, validDeleteRequest.OfficeId), Times.Once);
        }
    }

}

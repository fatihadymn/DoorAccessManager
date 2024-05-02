using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Authentication;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Exceptions;
using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace DoorAccessManager.Core.Services
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private JwtOptions _jwtOptions;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtOptions = _configuration.GetSection("jwt").Get<JwtOptions>()!;
            _mapper = mapper;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserAsync(request.Username, request.Password);

            if (user == null)
            {
                throw new BusinessException("User cannot be found");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim("officeId",user.OfficeId.ToString()),
                new Claim(ClaimTypes.Role,user.Role.Name)
            };

            var expiresTime = DateTime.Now.AddSeconds(Convert.ToDouble(_jwtOptions.ExpirySeconds));

            var jwt = new JwtSecurityToken(_jwtOptions.ValidIssuer,
                                           _jwtOptions.ValidIssuer,
                                           claims,
                                           expires: expiresTime,
                                           signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new LoginResponse()
            {
                Token = token
            };
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var result = await _userRepository.CreateUserAsync(new User()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Name = request.Name,
                OfficeId = request.OfficeId,
                PasswordHash = BC.HashPassword(request.Password),
                Username = request.Username,
            }, request.Role);

            return _mapper.Map<CreateUserResponse>(result);
        }

        public async Task<List<GetUserResponse>> GetUsersAsync(GetUsersRequest request)
        {
            var result = await _userRepository.GetUsersByOfficeIdAsync(request.OfficeId);

            return _mapper.Map<List<GetUserResponse>>(result);
        }

        public async Task UpdateUserPasswordAsync(UpdateUserPasswordRequest request)
        {
            if (request == null || request.UserId == Guid.Empty)
            {
                throw new BusinessException("Request is not valid");
            }

            await _userRepository.UpdateUserPasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
        }

        public async Task DeleteUserAsync(DeleteUserRequest request)
        {
            if (request == null || request.UserId == Guid.Empty || request.OfficeId == Guid.Empty)
            {
                throw new BusinessException("Request is not valid");
            }

            await _userRepository.DeleteUserAsync(request.UserId, request.OfficeId);
        }
    }
}

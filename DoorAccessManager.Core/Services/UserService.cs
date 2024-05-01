using DoorAccessManager.Core.Services.Abstract;
using DoorAccessManager.Data.Repositories.Abstract;
using DoorAccessManager.Items.Authentication;
using DoorAccessManager.Items.Exceptions;
using DoorAccessManager.Items.Models.Requests;
using DoorAccessManager.Items.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoorAccessManager.Core.Services
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private JwtOptions _jwtOptions;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtOptions = _configuration.GetSection("jwt").Get<JwtOptions>()!;
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
    }
}

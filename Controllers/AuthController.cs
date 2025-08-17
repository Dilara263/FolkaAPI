using FolkaAPI.Dtos;
using FolkaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = "1", Name = "Dilara", Email = "dilaraemail", Password = "123", PhoneNumber = "+90 555 123 4567", Address = "Folka Sanat Sokağı, No:12" },
            new User { Id = "2", Name = "emo", Email = "emoemail", Password = "123", PhoneNumber = null, Address = null }
        };

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto request)
        {
            if (_users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Bu email adresi zaten kullanılıyor." });
            }

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            };

            _users.Add(newUser);

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto request)
        {
            var user = _users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || user.Password != request.Password)
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            string token = CreateToken(user);
            return Ok(new { token = token, user = user });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public IActionResult UpdateProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (userEmail == null)
            {
                return Unauthorized();
            }

            var userToUpdate = _users.FirstOrDefault(u => u.Email == userEmail);

            if (userToUpdate == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Name))
            {
                userToUpdate.Name = userUpdateDto.Name;
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Email))
            {
                if (_users.Any(u => u.Email == userUpdateDto.Email && u.Id != userToUpdate.Id))
                {
                    return BadRequest("Bu e-posta adresi zaten kullanımda.");
                }
                userToUpdate.Email = userUpdateDto.Email;
            }

            if (userUpdateDto.PhoneNumber != null)
            {
                userToUpdate.PhoneNumber = userUpdateDto.PhoneNumber;
            }

            if (userUpdateDto.Address != null)
            {
                userToUpdate.Address = userUpdateDto.Address;
            }

            return Ok(userToUpdate);
        }
    }
}

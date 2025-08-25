using FolkaAPI.Dtos;
using FolkaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FolkaAPI.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Bu email adresi zaten kullanılıyor." });
            }

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            string token = CreateToken(user);
            return Ok(new
            {
                token = token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.PhoneNumber,
                    user.Address
                }
            });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
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
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userToUpdate == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Name))
            {
                userToUpdate.Name = userUpdateDto.Name;
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email && u.Id != userToUpdate.Id))
                {
                    return BadRequest(new { message = "Bu e-posta adresi zaten kullanımda." });
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

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                userToUpdate.Id,
                userToUpdate.Name,
                userToUpdate.Email,
                userToUpdate.PhoneNumber,
                userToUpdate.Address
            });
        }
    }
}

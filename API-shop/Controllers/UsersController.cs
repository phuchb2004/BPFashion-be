using API_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        private static class UserRoles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public UsersController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser req)
        {
            var isConflict = await _context.Users.AnyAsync(u => u.email == req.email || u.userName == req.userName);
            if (isConflict)
            {
                return Conflict(new { message = "Email hoặc tên đăng nhập đã được sử dụng" });
            }

            var user = new User
            {
                userName = req.userName,
                password = req.password,
                email = req.email,
                fullName = req.fullName,
                dob = req.dob,
                address = req.address,
                phone = req.phone,
                gender = req.gender,
                role = req.role,
                createAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserInfo), new { id = user.userId }, new
            {
                message = "Thêm người dùng thành công!",
                userId = user.userId
            });
        }

        [HttpPut("UpdateUser/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUser req)
        {
            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            var isConflict = await _context.Users.AnyAsync(u => u.userId != id && (u.email == req.email || u.userName == req.userName));
            if (isConflict)
            {
                return Conflict(new { message = "Email hoặc Tên đăng nhập đã được sử dụng bởi người dùng khác" });
            }

            userToUpdate.userName = req.userName;
            userToUpdate.email = req.email;
            userToUpdate.fullName = req.fullName;
            userToUpdate.dob = req.dob;
            userToUpdate.address = req.address;
            userToUpdate.phone = req.phone;
            userToUpdate.gender = req.gender;
            userToUpdate.role = req.role;

            if (!string.IsNullOrEmpty(req.password))
            {
                userToUpdate.password = req.password;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            if (userToDelete.role == UserRoles.Admin && userToDelete.userId == 1)
            {
                return BadRequest(new { message = "Không thể xóa admin chính" });
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa người dùng thành công" });
        }

        [HttpGet("GetUserInfo/{id}")]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            var user = await _context.Users
                .Select(u => new
                {
                    u.userId,
                    u.userName,
                    u.email,
                    u.fullName,
                    u.dob,
                    u.phone,
                    u.gender,
                    u.role,
                    u.address,
                    u.createAt
                })
                .FirstOrDefaultAsync(u => u.userId == id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(new { message = "Thông tin người dùng", user });
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.createAt)
                .Select(u => new
                {
                    u.userId,
                    u.userName,
                    u.email,
                    u.fullName,
                    u.dob,
                    u.phone,
                    u.gender,
                    u.role,
                    u.address,
                    u.createAt
                })
                .ToListAsync();

            return Ok(new { message = "Danh sách người dùng", users });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == loginRequest.email);

            if (user == null || loginRequest.password != user.password)
            {
                return Unauthorized(new { message = "Sai email hoặc mật khẩu" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim(ClaimTypes.Name, user.fullName),
                    new Claim(ClaimTypes.Role, user.role)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                message = $"Đăng nhập thành công! Chào mừng {user.fullName}",
                token = tokenString,
                user = new { user.userId, user.fullName, user.userName, user.email, user.role, user.phone, user.gender }
            });
        }
    }
}
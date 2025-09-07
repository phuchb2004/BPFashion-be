using API_shop.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(CreateUser req)
        {
            if (req == null)
            {
                return BadRequest("Invalid infomation");
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
                role = req.role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = $"Welcome {user.fullName} !!!"});
        }

        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUser req)
        {
            var userToUpdate = _context.Users.Find(id);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "No user found" });
            }

            userToUpdate.userName = req.userName;
            userToUpdate.email = req.email;
            userToUpdate.password = req.password;
            userToUpdate.fullName = req.fullName;
            userToUpdate.dob = req.dob;
            userToUpdate.address = req.address;
            userToUpdate.phone = req.phone;
            userToUpdate.gender = req.gender;
            userToUpdate.role = req.role;

            _context.SaveChanges();

            return Ok(new { message = $"{userToUpdate.fullName} is updated"});
        }

        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var userToDelete = _context.Users.Find(id);
            if (userToDelete == null)
            {
                return NotFound(new { message = "No user found" });
            }

            _context.Users.Remove(userToDelete);
            _context.SaveChanges();

            return Ok(new { message = "User Deleted" });
        }

        [HttpGet("GetUserInfo/{id}")]
        public IActionResult GetUserInfo(int id)
        {
            var userToGet = _context.Users.Find(id);
            if (userToGet == null)
            {
                return NotFound(new {message = "No user found" });
            }

            var userInfo = new User
            {
                userName = userToGet.userName,
                email = userToGet.email,
                password = userToGet.password
            };

            return Ok(new {message = $"{userToGet.fullName}'s infomation", 
                userToGet.userId, 
                userToGet.userName,
                userToGet.email, 
                userToGet.password, 
                userToGet.dob, 
                userToGet.phone, 
                userToGet.gender,
                userToGet.role
            });
        }

        [HttpGet("GetAllUser")]
        public IActionResult GetAllUser()
        {
            var user = _context.Users.ToList();
            if (user.Count == 0)
            {
                return NotFound(new { message = "List user is empty" });
            }

            return Ok(new { message = "All user infomation", user });
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == loginRequest.email);
            if (user == null || user.password != loginRequest.password)
            {
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu" });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.email),
                new Claim("role", user.role),
                new Claim("userId", user.userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hoangbaophucjoeytribbianimonkeydluffy15102004"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:7134",
                audience: "http://localhost:7134",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = $"Chào mừng {user.fullName}",
                token = tokenString,
                user = new
                {
                    userId = user.userId,
                    email = user.email,
                    password = user.password,
                    role = user.role
                }
            });
        }

        //[HttpPost("LoginGoogle")]
        //public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest req)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(req.Credential))
        //        {
        //            return BadRequest(new { message = "Thiếu Google token" });
        //        }

        //        var settings = new GoogleJsonWebSignature.ValidationSettings()
        //        {
        //            Audience = new[] { _config["Google:ClientId"] }
        //        };

        //        var payload = await GoogleJsonWebSignature.ValidateAsync(req.Credential, settings);

        //        if (payload == null)
        //        {
        //            return Unauthorized(new { message = "Google token không hợp lệ" });
        //        }

        //        var user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);
        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                email = payload.Email,
        //                fullName = payload.Name,
        //                password = null,
        //                role = "User",
        //                phone = null,
        //                //DOB = null,
        //                gender = null
        //            };

        //            _context.Users.Add(user);
        //            await _context.SaveChangesAsync();
        //        }

        //        var claims = new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        //            new Claim("role", user.Role),
        //            new Claim("fullname", user.FullName ?? ""),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //        };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: _config["Jwt:Issuer"],
        //            audience: _config["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.Now.AddHours(1),
        //            signingCredentials: creds
        //        );

        //        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //        return Ok(new
        //        {
        //            message = "Đăng nhập Google thành công",
        //            token = jwt,
        //            user = new
        //            {
        //                user.Id,
        //                user.Email,
        //                user.FullName,
        //                user.Role
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Lỗi đăng nhập Google", error = ex.Message });
        //    }
        //}

    }
}

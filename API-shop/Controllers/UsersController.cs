using API_shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
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
                Email = req.Email,
                Password = req.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User created", user.Email, user.Password });
        }

        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, UpdateUser req)
        {
            var userToUpdate = _context.Users.Find(id);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "No user found" });
            }

            userToUpdate.Email = req.Email;
            userToUpdate.Password = req.Password;

            _context.SaveChanges();

            return Ok(new { message = "User updated", userToUpdate.Email, userToUpdate.Password });
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
                Email = userToGet.Email,
                Password = userToGet.Password
            };

            return Ok(new {message = "User infomation", userToGet.Id, userToGet.Email, userToGet.Password});
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

    }
}

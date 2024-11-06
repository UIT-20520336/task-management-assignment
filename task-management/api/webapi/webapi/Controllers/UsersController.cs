using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UsersController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] LogInRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = _userService.GetUserByUsername(request.Username);
            if (existingUser != null)
            {
                return BadRequest("Username is already taken.");
            }

            _userService.RegisterUser(request.Username, request.Password);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LogInRequest request)
        {
            var user = _userService.GetUserByUsername(request.Username);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password.");
            }

            string token = _authService.GenerateJwtToken(user.Id.ToString(), user.Username);
            return Ok(new { Token = token });
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            var userlist = users.Select(user => new User
            {
                Id = user.Id,
                Username = user.Username
            }).ToList();

            return Ok(userlist);
        }
    }
}

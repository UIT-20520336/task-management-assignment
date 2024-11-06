using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _context;
        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _context = userService;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LogInRequest request)
        {
            var user = _context.GetUserByUsername(request.Username);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password.");
            }


            var token = _authService.GenerateJwtToken("user-id", request.Username);

            return Ok(new { Token = token });
        }

    }
}

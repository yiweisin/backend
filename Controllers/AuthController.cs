using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        
        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        
        [HttpPost("register")]
        public ActionResult<LoginResponseDto> Register(UserDto userDto)
        {
            if (_context.Users.Any(u => u.Username == userDto.Username))
            {
                return BadRequest("Username is taken");
            }
            
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = PasswordHasher.HashPassword(userDto.Password)
            };
            
            _context.Users.Add(user);
            _context.SaveChanges();
            
            var token = _tokenService.CreateToken(user);
            
            return new LoginResponseDto
            {
                Username = user.Username,
                Token = token
            };
        }
        
        [HttpPost("login")]
        public ActionResult<LoginResponseDto> Login(UserDto userDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == userDto.Username);
            
            if (user == null || !PasswordHasher.VerifyPassword(userDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }
            
            var token = _tokenService.CreateToken(user);
            
            return new LoginResponseDto
            {
                Username = user.Username,
                Token = token
            };
        }
    }
}
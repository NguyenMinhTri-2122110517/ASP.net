using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using NguyenMinhTri_2122110517.Services;
using Microsoft.AspNetCore.Cors;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public AuthController(IConfiguration configuration, AppDbContext context, IFileService fileService)
        {
            _configuration = configuration;
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Name == request.Name))
            {
                return Conflict(new { Message = "Tên đăng nhập đã tồn tại" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Conflict(new { Message = "Email đã được sử dụng" });
            }

            string avatarName = "default-avatar.png";
            if (request.Avatar != null)
            {
                avatarName = await _fileService.SaveFileAsync(request.Avatar, "avatars");
            }

            var newUser = new User
            {
                Name = request.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.Fullname,
                Gender = request.Gender,
                Phone = request.Phone,
                Email = request.Email,
                Roles = request.Roles ?? "customer",
                Avatar = avatarName,
                Address = request.Address,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow,
                Created_By = 0, // Bạn có thể thay bằng ID người tạo nếu có
                Status = 1
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(newUser);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Fullname = newUser.Fullname,
                    Email = newUser.Email,
                    Gender = newUser.Gender,
                    Roles = newUser.Roles,
                    Avatar = _fileService.GetFileUrl(newUser.Avatar, "avatars")
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Name == request.Name && u.Status == 1);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Gender = user.Gender,
                    Roles = user.Roles,
                    Avatar = _fileService.GetFileUrl(user.Avatar, "avatars")
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Roles ?? "customer"),
                new Claim("Fullname", user.Fullname ?? ""),
                new Claim("Gender", user.Gender ?? ""),
                new Claim("Avatar", user.Avatar ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public string Gender { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Address { get; set; }

        public string? Roles { get; set; }

        public IFormFile? Avatar { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
    }
}

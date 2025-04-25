using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using NguyenMinhTri_2122110517.Services;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public UserController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _context.Users.Where(u => u.Status == 1).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Status == 1);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromForm] UserCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Name and Password are required");
            }

            if (await _context.Users.AnyAsync(u => u.Name == request.Name))
            {
                return Conflict("Name already exists");
            }

            string avatarName = "default-avatar.png";
            if (request.Avatar != null)
            {
                avatarName = await _fileService.SaveFileAsync(request.Avatar, "avatars");
            }

            var user = new User
            {
                Name = request.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.Fullname,
                Gender = request.Gender,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Roles = request.Roles,
                Avatar = avatarName,
                Created_At = DateTime.UtcNow,
                Created_By = request.CreatedBy,
                Status = request.Status
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] UserUpdateRequest request)
        {
            if (id != request.Id) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (request.Avatar != null && user.Avatar != "default-avatar.png")
            {
                _fileService.DeleteFile(user.Avatar, "avatars");
                user.Avatar = await _fileService.SaveFileAsync(request.Avatar, "avatars");
            }

            user.Name = request.Name;
            user.Fullname = request.Fullname;
            user.Gender = request.Gender;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.Roles = request.Roles;
            user.Updated_At = DateTime.UtcNow;
            user.Updated_By = request.UpdatedBy;

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Avatar != "default-avatar.png")
            {
                _fileService.DeleteFile(user.Avatar, "avatars");
            }

            user.Status = 0;
            user.Updated_At = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class UserCreateRequest
        {
            [Required] public string Name { get; set; }
            [Required] public string Password { get; set; }
            public string Fullname { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Roles { get; set; }
            public IFormFile? Avatar { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; } = 1;
        }

        public class UserUpdateRequest
        {
            public int Id { get; set; }
            [Required] public string Name { get; set; }
            public string Password { get; set; }
            public string Fullname { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Roles { get; set; }
            public IFormFile? Avatar { get; set; }
            public int? UpdatedBy { get; set; }
        }
    }
}

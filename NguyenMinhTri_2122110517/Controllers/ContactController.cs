using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
        {
            return await _context.Contacts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound("Không tìm thấy liên hệ với ID này");
            }

            return contact;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> Create([FromBody] ContactCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newContact = new Contact
            {
                User_Id = request.User_Id,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Title = request.Title,
                Content = request.Content,
                Replay_Id = request.Replay_Id,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newContact.Id }, newContact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingContact = await _context.Contacts.FindAsync(id);
            if (existingContact == null)
            {
                return NotFound("Không tìm thấy liên hệ để cập nhật");
            }

            existingContact.User_Id = request.User_Id;
            existingContact.Name = request.Name;
            existingContact.Email = request.Email;
            existingContact.Phone = request.Phone;
            existingContact.Title = request.Title;
            existingContact.Content = request.Content;
            existingContact.Replay_Id = request.Replay_Id;
            existingContact.Updated_By = request.Updated_By ?? 0; // Gán giá trị mặc định là 0 nếu Updated_By là null
            existingContact.Updated_At = DateTime.UtcNow;
            existingContact.Status = request.Status;

            _context.Entry(existingContact).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Không tìm thấy liên hệ để xóa");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class ContactCreateRequest
        {
            public int User_Id { get; set; }
            [Required]
            public string Name { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int Replay_Id { get; set; }
            public int Status { get; set; }
        }

        public class ContactUpdateRequest
        {
            public int Id { get; set; }
            public int User_Id { get; set; }
            [Required]
            public string Name { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int Replay_Id { get; set; }
            public int? Updated_By { get; set; }
            public int Status { get; set; }
        }
    }
}
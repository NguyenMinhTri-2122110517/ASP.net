
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenMinhTri_2122110517.Data;
using NguyenMinhTri_2122110517.Model;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TopicController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetAll()
        {
            return await _context.Topics.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetById(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound("Không tìm thấy chủ đề với ID này");
            return topic;
        }

        [HttpPost]
        public async Task<ActionResult<Topic>> Create([FromBody] TopicCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newTopic = new Topic
            {
                Name = request.Name,
                Slug = request.Slug,
                Sort_Order = request.SortOrder,
                Description = request.Description,
                Created_By = request.CreatedBy,
                Created_At = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Topics.Add(newTopic);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newTopic.Id }, newTopic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TopicUpdateRequest request)
        {
            if (id != request.Id) return BadRequest();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound("Không tìm thấy chủ đề để cập nhật");

            topic.Name = request.Name;
            topic.Slug = request.Slug;
            topic.Sort_Order = request.SortOrder;
            topic.Description = request.Description;
            topic.Updated_By = request.UpdatedBy;
            topic.Updated_At = DateTime.UtcNow;
            topic.Status = request.Status;

            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound("Không tìm thấy chủ đề để xóa");

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class TopicCreateRequest
        {
            [Required] public string Name { get; set; }
            public string Slug { get; set; }
            public int SortOrder { get; set; }
            public string Description { get; set; }
            [Required] public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class TopicUpdateRequest
        {
            public int Id { get; set; }
            [Required] public string Name { get; set; }
            public string Slug { get; set; }
            public int SortOrder { get; set; }
            public string Description { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}
